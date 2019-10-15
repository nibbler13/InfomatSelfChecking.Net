using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Windows;

namespace InfomatSelfChecking {
	public sealed class ClientFirebird : IDisposable {
		private readonly FbConnectionStringBuilder connectionSB;
		private FbConnection connection;

//[yekuk_learn]
//server=172.16.9.90
//port=3050
//DATABASE=yekuk_learn
//name = FOR LEARNING
//comment = Версия 18.1 


        public ClientFirebird(string ipAddress, string baseName, string user, string pass) {
			connectionSB = new FbConnectionStringBuilder {
				DataSource = ipAddress,
				Database = baseName,
				UserID = user,
				Password = pass,
				Charset = "NONE",
				Pooling = false
			};

			CheckConnectionState();
		}



		public void Close() {
			connection.Close();
		}

		private void CheckConnectionState() {
			if (connection == null) {
				try {
					connection = new FbConnection(connectionSB.ToString());
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
					return;
				}
			}

			try {
				if (connection.State != ConnectionState.Open) {
					connection.Close();
					connection.Open();
				}
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}
		}

		public DataTable GetDataTable(string query, Dictionary<string, object> parameters) {
			Exception exc = new Exception();

			for (int i = 0; i < 3; i++) {
				try {
					Logging.ToLog("FirebirdClient.GetDataTable Attempt: " + (i + 1));
					CheckConnectionState();

					DataTable dataTable = new DataTable();
					using (FbCommand command = new FbCommand(query, connection)) {

						if (parameters != null && parameters.Count > 0)
							foreach (KeyValuePair<string, object> parameter in parameters)
								command.Parameters.AddWithValue(parameter.Key, parameter.Value);

						using (FbDataAdapter fbDataAdapter = new FbDataAdapter(command)) {
							fbDataAdapter.Fill(dataTable);
							return dataTable;
						}
					}
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
					Close();
					exc = e;
				}
			}

			throw exc;
		}

		public bool ExecuteUpdateQuery(string query, Dictionary<string, object> parameters) {
			Exception exc = new Exception();

			for (int i = 0; i < 3; i++) {
				try {
					Logging.ToLog("FirebirdClient.ExecuteUpdateQuery Attempt: " + (i + 1));
					CheckConnectionState();

					using (FbCommand update = new FbCommand(query, connection)) {
						if (parameters != null && parameters.Count > 0)
							foreach (KeyValuePair<string, object> parameter in parameters)
								update.Parameters.AddWithValue(parameter.Key, parameter.Value);

						return update.ExecuteNonQuery() >= 0;
					}

				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
					Close();
					exc = e;
				}
			}

			throw exc;
		}

		public void Dispose() {
			connection.Dispose();
		}
	}
}
