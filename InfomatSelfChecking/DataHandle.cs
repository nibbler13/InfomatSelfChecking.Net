using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
    public static class DataHandle {
		private static readonly string sqlGetPatients = Properties.Settings.Default.MisDbSqlGetPatients;
		private static readonly string sqlGetAppointments = Properties.Settings.Default.MisDbSqlGetAppointments;
		private static readonly string sqlGetDbState = Properties.Settings.Default.MisDbSqlCheckState;
		private static readonly string sqlSynchronizeMoscowClientInfo = Properties.Settings.Default.MisDbSqlSynchronizeMoscowClientInfo;
		private static readonly bool isThisAMoscowFilial;

		static DataHandle() {
			isThisAMoscowFilial = IsThisMoscowFilial();
		}

		private static readonly FirebirdClient fbClient = new FirebirdClient(
			Properties.Settings.Default.MisDbAddress,
			Properties.Settings.Default.MisDbName,
			Properties.Settings.Default.MisDbUser,
			Properties.Settings.Default.MisDbPassword);

		private static readonly FirebirdClient fbClientCentralDb = new FirebirdClient(
			Properties.Settings.Default.MisDbCentralAddress,
			Properties.Settings.Default.MisDbCentralName,
			Properties.Settings.Default.MisDbUser,
			Properties.Settings.Default.MisDbPassword);

		public static List<ItemPatient> GetPatients(string prefix, string number) {
			List<ItemPatient> patients = new List<ItemPatient>();

			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ "@prefix", prefix},
				{ "@number", number }};
			DataTable dataTable = fbClient.GetDataTable(sqlGetPatients, parameters);

			foreach (DataRow row in dataTable.Rows) {
				try {
                    ItemPatient itemPatient = new ItemPatient() {
                        PhoneNumber = prefix + number,
                        PCode = row["PCODE"].ToString(),
                        Name = row["CNAME"].ToString(),
                        Birthday = DateTime.Parse(row["BDATE"].ToString())
					};

					patients.Add(itemPatient);
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);

				}
			}

			return patients;
		}

		public static void UpdatePatientAppointments(ref ItemPatient patient) {
			Dictionary<string, object> parameters = new Dictionary<string, object> { { "@pCode", patient.PCode } };

			if (isThisAMoscowFilial && IsCentralDbAvailable())
				fbClient.ExecuteUpdateQuery(sqlSynchronizeMoscowClientInfo, parameters);

			DataTable dataTable = fbClient.GetDataTable(sqlGetAppointments, parameters);

			foreach (DataRow row in dataTable.Rows) {
				try {
					string group = row["GROUPS"].ToString().TrimStart(' ').TrimEnd(' ');
					string info = row["INFO"].ToString().TrimStart(' ').TrimEnd(' ');

					if (info.Equals("0"))
						continue;

					string[] infoData = info.Split('@');
					if (infoData.Length == 0)
						continue;

					switch (group) {
						case "_STOP_CODE":
							foreach (string code in infoData) {
								switch (code) {
									case "cash":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.Cash);
										break;
									case "firsttime":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.FirstTime);
										break;
									case "lock":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.Lock);
										break;
									case "late":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.Late);
										break;
									case "not_available_now":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.NotAvailableNow);
										break;
									case "depout":
										patient.StopCodesCurrent.Add(ItemPatient.StopCodes.DepOut);
										break;
									default:
										Logging.ToLog("Не удается распознать StopCode: " + code);
										break;
								}
							}
							break;
						case "_INFO_CODE":
							foreach (string code in infoData) {
								switch (code) {
									case "inform_about_lk":
										patient.InfoCodesCurrent.Add(ItemPatient.InfoCodes.InformAboutLK);
										break;
									default:
										Logging.ToLog("Не удается распознать InfoCode: " + code);
										break;
								}
							}
							break;
						default:
							if (infoData.Length != 6) {
								Logging.ToLog("Количество элементов в строке не равно 6: " + info);
								break;
							}

							ItemAppointment itemAppointment = new ItemAppointment() {
								DateTimeSchedule = infoData[0],
								DepShortName = infoData[1],
								DepName = infoData[2],
								DName = infoData[3],
								RNum = infoData[4],
								SchedID = infoData[5]
							};

							switch (group) {
								case "visited":
									patient.AppointmentsVisited.Add(itemAppointment);
									break;
								case "available":
									patient.AppointmentsAvailable.Add(itemAppointment);
									break;
								case "not_available":
									patient.AppointmentsNotAvailable.Add(itemAppointment);
									break;
								default:
									break;
							}

							break;
					}

				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				}
			}
		}

		private static bool IsCentralDbAvailable() {
			return fbClientCentralDb.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count > 0;
		}

		public static void CheckDbAvailable() {
            if (fbClient.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count == 0)
                throw new Exception("DataHandle - CheckDbAvailable - result is empty");
		}

		public static bool IsThisMoscowFilial() {
			return fbClient.GetDataTable(Properties.Settings.Default.MisDbSqlCheckIfMoscowFilial, 
				new Dictionary<string, object>()).Rows[0][0].ToString().Equals("1");
		}
    }
}
