using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfomatSelfChecking.Items;

namespace InfomatSelfChecking {
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
	public class DataHandle {
#pragma warning restore CA1001 // Types that own disposable fields should be disposable

		private static DataHandle instance = null;
		private static readonly object padlock = new object();

		public static DataHandle Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new DataHandle();

					return instance;
				}
			}
		}

		private readonly string sqlGetPatients = Properties.Settings.Default.MisDbSqlGetPatients;
		private readonly string sqlGetAppointments = Properties.Settings.Default.MisDbSqlGetAppointments;
		private readonly string sqlGetDbState = Properties.Settings.Default.MisDbSqlCheckDbState;
		private readonly string sqlSynchronizeMoscowClientInfo = Properties.Settings.Default.MisDbSqlSynchronizeMoscowClientInfo;
		private readonly string sqlUpdateAppointment = Properties.Settings.Default.MisDbSqlUpdateAppointments;
		private readonly bool isThisAMoscowFilial;
		private readonly bool isThisASpbFilial;

		private readonly ClientFirebird fbClient;
		private readonly ClientFirebird fbClientCentralDb;

		private DataHandle() {
			fbClient = new ClientFirebird(
				Properties.Settings.Default.MisDbAddress,
				Properties.Settings.Default.MisDbName,
				Properties.Settings.Default.MisDbUser,
				Properties.Settings.Default.MisDbPassword);

			fbClientCentralDb = new ClientFirebird(
				Properties.Settings.Default.MisDbCentralAddress,
				Properties.Settings.Default.MisDbCentralName,
				Properties.Settings.Default.MisDbUser,
				Properties.Settings.Default.MisDbPassword);

			isThisAMoscowFilial = IsCurrentFilialInList(new string[] { "1", "5", "12" });
			isThisASpbFilial = IsCurrentFilialInList(new string[] { "3" });
		}

		public static List<ItemPatient> PatientsCurrent { get; private set; } = new List<ItemPatient>();

		public bool SetCheckInForAppointments(List<string> schedIds) {
			if (schedIds == null)
				throw new ArgumentNullException(nameof(schedIds));

			Logging.ToLog("DataHandle - установка отметок о посещении для следующих назначений SchedID: " +
				string.Join(", ", schedIds));

			bool result = true;

			if (Debugger.IsAttached)
				return result;

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

		public void LoadPatients(string prefix, string number) {
			Logging.ToLog("DataHandle - получения списка пациентов по введенному номеру: " + prefix + number);

			PatientsCurrent.Clear();

			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ "@prefix", prefix},
				{ "@number", number }};

			try {
				using (DataTable dataTable = fbClient.GetDataTable(sqlGetPatients, parameters)) {
					foreach (DataRow row in dataTable.Rows) {
						try {
							ItemPatient itemPatient = new ItemPatient() {
								PhoneNumber = prefix + number,
								PCode = row["PCODE"].ToString(),
								Name = row["CNAME"].ToString(),
								Birthday = DateTime.Parse(row["BDATE"].ToString())
							};

							GetPatientAppointments(ref itemPatient);

							PatientsCurrent.Add(itemPatient);
						} catch (Exception e) {
							Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
						}
					}
				}
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				MainWindow.Instance.ShowErrorScreen(e, true);
				return;
			}

			Logging.ToLog("DataHandle - полученный список пациентов: " + 
				string.Join(Environment.NewLine, PatientsCurrent));
		}

		private void GetPatientAppointments(ref ItemPatient patient) {
			Logging.ToLog("DataHandle - получение списка назнчений для пациента. PCODE: " + patient.PCode + ", ФИО: " + patient.Name);

			Dictionary<string, object> parameters = new Dictionary<string, object> { { "@pCode", patient.PCode } };

			if (isThisAMoscowFilial && IsCentralDbAvailable()) {
				Logging.ToLog("DataHandle - Синхронизация данных с ЦБД");
				try {
					fbClient.ExecuteUpdateQuery(sqlSynchronizeMoscowClientInfo, parameters);
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.StackTrace + e.StackTrace);
				}
			}

			try {
				using (DataTable dataTable = fbClient.GetDataTable(sqlGetAppointments, parameters)) 
					foreach (DataRow row in dataTable.Rows) 
						ParsePatientAppointmentRow(row, ref patient);
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				MainWindow.Instance.ShowErrorScreen(e, true);
				return;
			}


			List<ItemAppointment> appointmentSorted = patient.AppointmentsAvailable.OrderBy(x => x.DateTimeScheduleBegin).ToList();
			patient.AppointmentsAvailable.Clear();
			patient.AppointmentsAvailable.AddRange(appointmentSorted);

			if (patient.StopCodesCurrent.Count == 0 &&
				patient.AppointmentsAvailable.Count + patient.AppointmentsVisited.Count > 0)
				patient.CheckPrinterAndCreateWorksheet();
		}

		private void ParsePatientAppointmentRow(DataRow row, ref ItemPatient patient) {
			try {
				string group = row["GROUPS"].ToString().TrimStart(' ').TrimEnd(' ');
				string info = row["INFO"].ToString().TrimStart(' ').TrimEnd(' ');

				if (info.Equals("0"))
					return;

				string[] infoData = info.Split('@');
				if (infoData.Length == 0)
					return;

				switch (group) {
					case "_STOP_CODE":
						foreach (string code in infoData) {
							switch (code) {
								case "cash":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.Cash);
									break;
								case "firsttime":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.FirstTime);
									break;
								case "lock":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.Lock);
									break;
								case "late":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.Late);
									break;
								//case "not_available_now":
								//	patient.StopCodesCurrent.Add(ItemPatient.StopCodes.NotAvailableNow);
								//	break;
								case "depout":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.DepOut);
									break;
								case "debt":
									patient.StopCodesCurrent.Add(ItemPatient.StopCode.Debt);
									break;
								case "sogl":
									if (isThisASpbFilial)
										patient.StopCodesCurrent.Add(ItemPatient.StopCode.Agreement);
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
									patient.InfoCodesCurrent.Add(ItemPatient.InfoCode.InformAboutLK);
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
								itemAppointment.AlreadyChecked = true;
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

		private bool IsCentralDbAvailable() {
			Logging.ToLog("DataHandle - проверка доступности ЦБД");
			using (DataTable dataTable = fbClientCentralDb.GetDataTable(sqlGetDbState)) {
				bool result = dataTable.Rows.Count > 0;
				Logging.ToLog("DataHandle - возвращаемое значение: " + result);
				return result;
			}
		}

		public void CheckDbAvailable() {
			Logging.ToLog("DataHandle - проверка доступности БД");
			using (DataTable dataTable = fbClient.GetDataTable(sqlGetDbState)) 
				if (dataTable.Rows.Count == 0)
					throw new Exception("DataHandle - CheckDbAvailable - result is empty");
		}

		public bool IsCurrentFilialInList(string[] filialsID) {
			Logging.ToLog("DataHandle - определение принадлежности филиала к списку: " + string.Join(",", filialsID));

			try {
				using (DataTable dataTable = fbClient.GetDataTable(Properties.Settings.Default.MisDbSqlGetFilialID)) {
					string filID = dataTable.Rows[0][0].ToString();
					bool result = filialsID.Contains(filID);
					Logging.ToLog("DataHandle - возвращаемое значение: " + result);
					return result;
				}
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				return false;
			}
		}

		public void CloseDbConnections() {
			fbClient.Dispose();
			fbClientCentralDb.Dispose();
		}
    }
}
