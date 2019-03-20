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
		private static readonly string sqlGetDbState = Properties.Settings.Default.MisDbSqlCheckDbState;
		private static readonly string sqlSynchronizeMoscowClientInfo = Properties.Settings.Default.MisDbSqlSynchronizeMoscowClientInfo;
		private static readonly string sqlUpdateAppointment = Properties.Settings.Default.MisDbSqlUpdateAppointments;
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

		public static bool SetCheckInForAppointments(List<string> schedIds) {
			Logging.ToLog("DataHandle - установка отметок о посещении для следующих назначений SchedID: " +
				string.Join(", ", schedIds));

			bool result = true;

			foreach (string schedId in schedIds)
				try {
					if (!fbClient.ExecuteUpdateQuery(sqlUpdateAppointment,
						new Dictionary<string, object> { { "@schedId", schedId } }))
						result = false;
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
					result = false;
				}

			return result;
		}

		public static List<ItemPatient> GetPatients(string prefix, string number) {
			Logging.ToLog("DataHandle - получения списка пациентов по введенному номеру: " + prefix + number);

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

					GetPatientAppointments(ref itemPatient);

					patients.Add(itemPatient);
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);

				}
			}

			Logging.ToLog("DataHandle - полученный список пациентов: " + 
				string.Join(Environment.NewLine, patients));

			return patients;
		}

		private static void GetPatientAppointments(ref ItemPatient patient) {
			Logging.ToLog("DataHandle - получение списка назнчений для пациента. PCODE: " + patient.PCode + ", ФИО: " + patient.Name);

			Dictionary<string, object> parameters = new Dictionary<string, object> { { "@pCode", patient.PCode } };

			if (isThisAMoscowFilial && IsCentralDbAvailable()) {
				Logging.ToLog("DataHandle - Синхронизация данных с ЦБД");
				fbClient.ExecuteUpdateQuery(sqlSynchronizeMoscowClientInfo, parameters);
			}

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
                                    case "debt":
                                        patient.StopCodesCurrent.Add(ItemPatient.StopCodes.Debt);
                                        break;
									case "":
										break;
									default:
										Logging.ToLog("DataHandle - Не удается распознать StopCode: " + code);
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
									case "":
										break;
									default:
										Logging.ToLog("DataHandle - Не удается распознать InfoCode: " + code);
										break;
								}
							}
							break;
						default:
							if (infoData.Length != 7) {
								Logging.ToLog("DataHandle - Количество элементов в строке не равно 6: " + info);
								break;
							}

							ItemAppointment itemAppointment = new ItemAppointment() {
								DateTimeScheduleBegin = infoData[0],
								DateTimeScheduleEnd = infoData[1],
                                DepShortName = infoData[2],
								DepName = ControlsFactory.FirstCharToUpper(infoData[3]),
								DName = infoData[4],
								RNum = infoData[5].Replace('№', ' '),
								SchedID = infoData[6]
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

			patient.AppointmentsAvailable = 
				patient.AppointmentsAvailable.OrderBy(x => x.DateTimeScheduleBegin).ToList();

			if (patient.StopCodesCurrent.Count == 0 &&
				patient.AppointmentsAvailable.Count > 0)
				patient.CheckPrinterAndCreateWorksheet();
		}

		private static bool IsCentralDbAvailable() {
			Logging.ToLog("DataHandle - проверка доступности ЦБД");
			bool result = fbClientCentralDb.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count > 0;
			Logging.ToLog("DataHandle - возвращаемое значение: " + result);
			return result;
		}

		public static void CheckDbAvailable() {
			Logging.ToLog("DataHandle - проверка доступности БД");
            if (fbClient.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count == 0)
                throw new Exception("DataHandle - CheckDbAvailable - result is empty");
		}

		public static bool IsThisMoscowFilial() {
			Logging.ToLog("DataHandle - определение принадлежности филиала к МСК");
			bool result = fbClient.GetDataTable(Properties.Settings.Default.MisDbSqlCheckIfMoscowFilial,
				new Dictionary<string, object>()).Rows[0][0].ToString().Equals("1");
			Logging.ToLog("DataHandle - возвращаемое значение: " + result);
			return result;
		}
    }
}
