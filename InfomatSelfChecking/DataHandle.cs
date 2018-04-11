using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
    public class DataHandle {
		private static string sqlGetPatients = Properties.Settings.Default.MisDbSqlGetPatients;
		private static string sqlGetAppointments = Properties.Settings.Default.MisDbSqlGetAppointments;
		private static string sqlGetDbState = Properties.Settings.Default.MisDbSqlCheckState;
		private static FirebirdClient fbClient = new FirebirdClient(
			Properties.Settings.Default.MisDbAddress,
			Properties.Settings.Default.MisDbName,
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
						FirstName = row["FIRSTNAME"].ToString(),
						MiddleName = row["MIDNAME"].ToString(),
						Birthday = row["BDATE"].ToString(),
						IsFirstVisit = row["FIRSTVISIT"].ToString().Equals("1") ? true : false,
						IsCardBlocked = row["CARDBLOCKED"].ToString().Equals("1") ? true : false,
						HasOnlineAccount = row["LK"].ToString().Equals("1") ? true : false,
						Sex = int.Parse(row["POL"].ToString())
					};

					patients.Add(itemPatient);
				} catch (Exception e) {
					Logging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			if (Debugger.IsAttached) {
				patients.Add(new ItemPatient() {
					PhoneNumber = prefix + number,
					PCode = "00000001",
					FirstName = "Павел",
					MiddleName = "Павлович",
					Birthday = "21.10.1987",
					IsFirstVisit = false,
					IsCardBlocked = false,
					HasOnlineAccount = false,
					Sex = 0
				});

				//patients.Add(new ItemPatient() {
				//	PhoneNumber = prefix + number,
				//	PCode = "00000001",
				//	FirstName = "Сергей",
				//	MiddleName = "Анатольевич",
				//	Birthday = "01.01.1989",
				//	IsFirstVisit = true,
				//	IsCardBlocked = false,
				//	HasOnlineAccount = false,
				//	Sex = 0
				//});

				//patients.Add(new ItemPatient() {
				//	PhoneNumber = prefix + number,
				//	PCode = "00000001",
				//	FirstName = "Майя",
				//	MiddleName = "Николаевна",
				//	Birthday = "25.05.1992",
				//	IsFirstVisit = false,
				//	IsCardBlocked = true,
				//	HasOnlineAccount = false,
				//	Sex = 1
				//});

				//patients.Add(new ItemPatient() {
				//	PhoneNumber = prefix + number,
				//	PCode = "00000001",
				//	FirstName = "Тамара",
				//	MiddleName = "Павловна",
				//	Birthday = "11.05.1959",
				//	IsFirstVisit = false,
				//	IsCardBlocked = false,
				//	HasOnlineAccount = false,
				//	Sex = 1
				//});
			}

			return patients;
		}

		public static List<ItemAppointment> GetAppointments(string pcode) {
			List<ItemAppointment> appointments = new List<ItemAppointment>();

			Dictionary<string, object> parameters = new Dictionary<string, object> { { "@pcode", pcode } };
			DataTable dataTable = fbClient.GetDataTable(sqlGetAppointments, parameters);

			foreach (DataRow row in dataTable.Rows) {
				try {
					ItemAppointment itemAppointment = new ItemAppointment() {
						SchedID = row["SCHEDID"].ToString(),
						DateTimeBegin = DateTime.Parse(
							row["WORKDATE"].ToString() + " " +
							row["BHOUR"].ToString() + ":" +
							row["BMIN"].ToString()),
						DName = row["DNAME"].ToString(),
						DepName = row["DEPNAME"].ToString(),
						RNum = row["RNUM"].ToString(),
						IsCash = row["ISCASH"].ToString().Equals("1") ? true : false,
						IsRoentgen = row["ISROENTGEN"].ToString().Equals("1") ? true : false,
						ClVisit = row["CLVISIT"].ToString()
					};

					appointments.Add(itemAppointment);
				} catch (Exception e) {
					Logging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			if (Debugger.IsAttached) {
				appointments.Add(new ItemAppointment() {
					SchedID = "000001",
					DateTimeBegin = DateTime.Parse("27.03.2018 15:00"),
					DName = "Проверкин П.П.",
					DepName = "Терапия",
					DepShortName = "ТЕР",
					RNum = "№201",
					IsLate = false,
					IsCash = false,
					IsRoentgen = false,
					ClVisit = "",
				});

				//Random random = new Random();
				//bool isCash = random.Next(0, 2) == 0 ? true : false;
				//bool isLage = random.Next(0, 2) == 0 ? true : false;
				//bool isRoentgen = random.Next(0, 2) == 0 ? true : false;
				//appointments.Add(new ItemAppointment() {
				//	SchedID = "000002",
				//	DateTimeBegin = DateTime.Parse("27.03.2018 15:30"),
				//	DName = "Алехина С.П.",
				//	DepName = "Хирургия",
				//	DepShortName = "ХРГ",
				//	RNum = "№204",
				//	IsLate = isLage,
				//	IsCash = isCash,
				//	IsRoentgen = isRoentgen,
				//	ClVisit = "",
				//});

				//appointments.Add(new ItemAppointment() {
				//	SchedID = "000002",
				//	DateTimeBegin = DateTime.Parse("27.03.2018 16:00"),
				//	DName = "ОченьДлиннаяФамилияДоктора-ПлюсЕщеЧастьФамилии П.П.",
				//	DepName = "Функциональная диагностика",
				//	DepShortName = "ФДИАГ",
				//	RNum = "№501",
				//	IsLate = false,
				//	IsCash = false,
				//	IsRoentgen = false,
				//	ClVisit = "",
				//});

				//appointments.Add(new ItemAppointment() {
				//	SchedID = "000002",
				//	DateTimeBegin = DateTime.Parse("27.03.2018 16:30"),
				//	DName = "Иванов И.И.",
				//	DepName = "Неврология",
				//	DepShortName = "НЕВР",
				//	RNum = "№501",
				//	IsLate = false,
				//	IsCash = false,
				//	IsRoentgen = false,
				//	ClVisit = "",
				//});

				//appointments.Add(new ItemAppointment() {
				//	SchedID = "000002",
				//	DateTimeBegin = DateTime.Parse("27.03.2018 17:00"),
				//	DName = "Петров П.П.",
				//	DepName = "Массаж",
				//	DepShortName = "МСЖ",
				//	RNum = "№501",
				//	IsLate = false,
				//	IsCash = false,
				//	IsRoentgen = false,
				//	ClVisit = "",
				//});
			}
			
			return appointments;
		}

		public static bool IsDbAlive() {
			return fbClient.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count > 0;
		}
    }
}
