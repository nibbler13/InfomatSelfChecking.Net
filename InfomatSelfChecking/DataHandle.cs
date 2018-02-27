using System;
using System.Collections.Generic;
using System.Data;
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
			List<ItemPatient> list = new List<ItemPatient>();

			Dictionary<string, object> parameters = new Dictionary<string, object> {
				{ "@prefix", prefix},
				{ "@number", number }};
			DataTable dataTable = fbClient.GetDataTable(sqlGetPatients, parameters);

			foreach (DataRow row in dataTable.Rows) {
				try {
					ItemPatient itemPatient = new ItemPatient() {
						PhoneNumber = prefix + number,
						PCode = row["PCODE"].ToString(),
						Firstname = row["FIRSTNAME"].ToString(),
						MiddleName = row["MIDNAME"].ToString(),
						Birthday = row["BDATE"].ToString(),
						IsFirstVisit = row["FIRSTVISIT"].ToString(),
						IsCardBlocked = row["CARDBLOCKED"].ToString(),
						HasOnlineAccount = row["LK"].ToString(),
						Sex = row["POL"].ToString()
					};

					list.Add(itemPatient);
				} catch (Exception e) {
					Logging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}

			return list;
		}

		public static List<ItemAppointment> GetAppointments(string pcode) {
			List<ItemAppointment> list = new List<ItemAppointment>();

			Dictionary<string, object> parameters = new Dictionary<string, object> { { "@pcode", pcode } };
			DataTable dataTable = fbClient.GetDataTable(sqlGetAppointments, parameters);

			foreach (DataRow row in dataTable.Rows) {
				try {
					ItemAppointment itemAppointment = new ItemAppointment() {
						SchedID = row["SCHEDID"].ToString(),
						WorkDate = row["WORKDATE"].ToString(),
						BHour = row["BHOUR"].ToString(),
						BMin = row["BMIN"].ToString(),
						DName = row["DNAME"].ToString(),
						DepName = row["DEPNAME"].ToString(),
						RNum = row["RNUM"].ToString(),
						Kateg = row["KATEG"].ToString(),
						ClVisit = row["CLVISIT"].ToString(),
						DepNum = row["DEPNUM"].ToString()
					};

					list.Add(itemAppointment);
				} catch (Exception e) {
					Logging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
				}
			}
			
			return list;
		}

		public static bool IsDbAlive() {
			return fbClient.GetDataTable(sqlGetDbState, new Dictionary<string, object>()).Rows.Count > 0;
		}
    }
}
