using System;
using System.Data;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Windows;

namespace InfomatSelfChecking {
	public class FirebirdClient {
		private readonly FbConnection connection;

//[yekuk_learn]
//server=172.16.9.90
//port=3050
//DATABASE=yekuk_learn
//name = FOR LEARNING
//comment = Версия 18.1 


        public FirebirdClient(string ipAddress, string baseName, string user, string pass) {
			FbConnectionStringBuilder cs = new FbConnectionStringBuilder {
				DataSource = ipAddress,
				Database = baseName,
				UserID = user,
				Password = pass,
				Charset = "NONE",
				Pooling = false
			};

			connection = new FbConnection(cs.ToString());
			CheckConnectionState();
		}

		public void Close() {
			connection.Close();
		}

		private void CheckConnectionState() {
			if (connection.State != ConnectionState.Open) 
					connection.Open();
		}

		public DataTable GetDataTable(string query, Dictionary<string, object> parameters) {
            CheckConnectionState();

            DataTable dataTable = new DataTable();
			FbCommand command = new FbCommand(query, connection);

            if (parameters.Count > 0)
                foreach (KeyValuePair<string, object> parameter in parameters)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);

            FbDataAdapter fbDataAdapter = new FbDataAdapter(command);
			fbDataAdapter.Fill(dataTable);

			return dataTable;
		}

		public bool ExecuteUpdateQuery(string query, Dictionary<string, object> parameters) {
            CheckConnectionState();

            FbCommand update = new FbCommand(query, connection);

            if (parameters.Count > 0)
                foreach (KeyValuePair<string, object> parameter in parameters)
                    update.Parameters.AddWithValue(parameter.Key, parameter.Value);

            return update.ExecuteNonQuery() >= 0 ? true : false; ;
		}
	}
}
