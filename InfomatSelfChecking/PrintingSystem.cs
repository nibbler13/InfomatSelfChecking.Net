using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace InfomatSelfChecking {
	class PrintingSystem {
		private static PrintingSystem instance = null;
		private static readonly object padlock = new object();
		public static PrintingSystem Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new PrintingSystem();

					return instance;
				}
			}
		}

		private Excel.Application xlApp = null;
		private Excel.Workbook xlWb = null;
		private Excel.Worksheet xlWs = null;
		private readonly string templateFullPath;
		private readonly string saveFolder;

		private const int ROW_DATE_TIME = 4;
		private const int ROW_NAME = 5;
		private const int ROW_FAMILY = 6;
		private const int ROW_STYLE = 7;
		private const int ROW_START = 9;

		public PrintingSystem() {
			Logging.ToLog("Запуск Excel");
			xlApp = new Excel.Application();
			if (xlApp == null) { 
				Logging.ToLog("Не удалось открыть приложение Excel");
				return;
			}

			xlApp.Visible = false;
			string currentDir = Directory.GetCurrentDirectory();
			templateFullPath = Path.Combine(currentDir, "ExcelTemplate", "PrintTemplate.xlsx");
			if (!File.Exists(templateFullPath)) {
				Logging.ToLog("Не удалось получить доступ к файлу шаблона: " + templateFullPath);
				return;
			}

			saveFolder = Path.Combine(currentDir, "Results");
			if (!Directory.Exists(saveFolder)) {
				try {
					Directory.CreateDirectory(saveFolder);
				} catch (Exception e) {
					saveFolder = string.Empty;
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				}
			}
		}

		private bool OpenTemplate() {
			try {
				Logging.ToLog("Открытие книги: " + templateFullPath);
				xlWb = xlApp.Workbooks.Open(templateFullPath, ReadOnly: true);

				if (xlWb == null) {
					Logging.ToLog("Не удалось открыть книгу: " + templateFullPath);
					return false;
				}

				Logging.ToLog("Открытие листа: Template");
				xlWs = xlWb.Sheets["Template"];
				if (xlWs == null) {
					Logging.ToLog("Не удалось открыть лист: Template");
					return false;
				}

				return true;
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			return false;
		}

		public async void PrintAppointments(ItemPatient patient) {
			Logging.ToLog("Печать назначений для пациента: " + patient.Name + ", pcode: " + patient.PCode);
			if (xlApp == null) {
				Logging.ToLog("Не запущен Excel, пропуск");
				return;
			}

			await Task.Run(() => {
				if (!OpenTemplate())
					return;

				string[] nameSplitted = patient.Name.Split(' ');
				string name = nameSplitted[0];
				string family = patient.Name.Replace(name + " ", "");

				xlWs.Range["A" + ROW_NAME].Value2 = name;
				xlWs.Range["A" + ROW_FAMILY].Value2 = family + ",";
				xlWs.Range["A" + ROW_DATE_TIME].Value2 = 
				DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString();

				CloseWorkbook(patient.Name + "_" + patient.PhoneNumber);
			});
		}

		private void WriteInfoToFile() {

		}

		private void CloseWorkbook(string filePostFix = "") {
			if (!string.IsNullOrEmpty(saveFolder) &&
				!string.IsNullOrEmpty(filePostFix)) {
				xlWb.SaveAs(Path.Combine(saveFolder, "PrintResult_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + filePostFix));
			}

			if (xlWs != null) {
				Marshal.ReleaseComObject(xlWs);
				xlWs = null;
			}

			if (xlWb != null) {
				xlWb.Close(false);
				Marshal.ReleaseComObject(xlWb);
				xlWb = null;
			}
		}

		public void CloseExcel() {
			CloseWorkbook();

			if (xlApp != null) {
				xlApp.Quit();
				Marshal.ReleaseComObject(xlApp);
				xlApp = null;
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
	}
}
