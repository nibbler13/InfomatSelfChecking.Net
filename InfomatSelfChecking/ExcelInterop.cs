using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace InfomatSelfChecking {
	class ExcelInterop {
		private static ExcelInterop instance = null;
		private static readonly object padlock = new object();
		public static ExcelInterop Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new ExcelInterop();

					return instance;
				}
			}
		}

		private Excel.Application xlApp = null;
		private readonly string templateFullPath;
		private readonly string saveFolder;
		private readonly string clinic_address;
		private readonly string clinic_phone_number;

		private const int ROW_CLINIC_ADDRESS = 2;
		private const int ROW_CLINIC_PHONE_NUMBER = 3;
		private const int ROW_DATE_TIME = 4;
		private const int ROW_NAME = 5;
		private const int ROW_FAMILY = 6;
		private const int ROW_STYLE = 7;
		private const int ROW_DELIMITER = 8;
		private const int ROW_START = 9;

		public ExcelInterop() {
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

			clinic_address = Properties.Settings.Default.ClinicAddress;
			clinic_phone_number = Properties.Settings.Default.ClinicPhoneNumber;
		}

		private Excel.Worksheet OpenTemplate(out Excel.Workbook wb) {
			try {
				Logging.ToLog("Открытие книги: " + templateFullPath);
				wb = xlApp.Workbooks.Open(templateFullPath, ReadOnly: true);

				if (wb == null) {
					Logging.ToLog("Не удалось открыть книгу: " + templateFullPath);
					return null;
				}

				Logging.ToLog("Открытие листа: Template");
				Excel.Worksheet ws = wb.Sheets["Template"];
				if (ws == null) {
					Logging.ToLog("Не удалось открыть лист: Template");
					return null;
				}

				return ws;
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			wb = null;
			return null;
		}

		public Excel.Worksheet CreateWorksheetAppointmentsAvailable(ItemPatient patient, out Excel.Workbook wb) {
			Logging.ToLog("Печать назначений для пациента: " + patient.Name + ", pcode: " + patient.PCode);
			wb = null;

			if (xlApp == null) {
				Logging.ToLog("Не запущен Excel, пропуск");
				return null;
			}

			Excel.Worksheet ws = OpenTemplate(out wb);
			if (ws == null)
				return null;

			Logging.ToLog("Запись информации о назначениях");
			string[] nameSplitted = patient.Name.Split(' ');
			string name = nameSplitted[0];
			string family = patient.Name.Replace(name + " ", "");

			SetValue(ws, ROW_CLINIC_ADDRESS, clinic_address);
			SetValue(ws, ROW_CLINIC_PHONE_NUMBER, clinic_phone_number);
			SetValue(ws, ROW_NAME, name);
			SetValue(ws, ROW_FAMILY, family + ",");
			SetValue(ws, ROW_DATE_TIME, DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString());

			int currentRow = ROW_START;
			foreach (ItemAppointment item in patient.AppointmentsAvailable) {
				SetValue(ws, currentRow, item.DateTimeScheduleBegin + ", кабинет " + item.RNum);
				currentRow++;
				SetValue(ws, currentRow, item.DName);
				currentRow++;
				SetValue(ws, currentRow, item.DepName);

				PasteFormat(ws, ROW_START, currentRow - 2, ROW_START + 2, currentRow);
				PasteDelimiter(ws, ref currentRow);
			}

			string timeMessage = Properties.Resources.print_message_final_ok;
			int timeLeft = patient.AppointmentsAvailable.First().GetMinutesLeftToBegin();
			if (timeLeft >= 10)
				timeMessage = Properties.Resources.print_message_time_left + timeLeft +
					" " + Helper.GetDeclension(timeLeft);

			SetValue(ws, currentRow, timeMessage);
			PasteFormat(ws, ROW_STYLE, currentRow);
			PasteDelimiter(ws, ref currentRow);

			string notificationMessage = Properties.Resources.print_message_information_loyalty;
			if (patient.InfoCodesCurrent.Contains(ItemPatient.InfoCodes.InformAboutLK)) {
				Random rnd = new Random();
				if (rnd.Next(0, 100) < 50)
					notificationMessage = Properties.Resources.print_message_information_private_office;
			}

			SetValue(ws, currentRow, notificationMessage);
			PasteFormat(ws, ROW_STYLE, currentRow);

			Console.WriteLine("-----Возврат созданного листа-----");

			return ws;
		}

		private void SetValue(Excel.Worksheet ws, int row, string value) {
			ws.Range["A" + row].Value2 = value;
		}

		private void PasteFormat(Excel.Worksheet ws, int rowStyleStart, int rowPasteStart, int rowStyleEnd = -1, int rowPasteEnd = -1) {
			if (rowStyleEnd == -1)
				rowStyleEnd = rowStyleStart;

			if (rowPasteEnd == -1)
				rowPasteEnd = rowPasteStart;

			ws.Range["A" + rowStyleStart + ":A" + rowStyleEnd].Select();
			xlApp.Selection.Copy();
			ws.Range["A" + rowPasteStart + ":A" + rowPasteEnd].Select();
			xlApp.Selection.PasteSpecial(Excel.XlPasteType.xlPasteFormats);
		}

		private void PasteDelimiter(Excel.Worksheet ws, ref int currentRow) {
			currentRow++;
			ws.Range["A" + ROW_DELIMITER].Select();
			xlApp.Selection.Copy();
			ws.Range["A" + currentRow].Select();
			ws.Paste();
			currentRow++;
		}

		public void PrintWorksheetAndCloseWorkbook(ref Excel.Worksheet ws, ref Excel.Workbook wb, string filePostfix) {
			Console.WriteLine("--- печать листа Excel");
			ws.PrintOutEx();
			CloseWorkbook(ref ws, ref wb, filePostfix);
		}

		private void CloseWorkbook(ref Excel.Worksheet ws, ref Excel.Workbook wb, string filePostfix) {
			Console.WriteLine("--- сохранение книги Excel");
			if (!string.IsNullOrEmpty(saveFolder))
				wb.SaveAs(Path.Combine(saveFolder, "PrintResult_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + filePostfix));

			if (ws != null) {
				Marshal.ReleaseComObject(ws);
				ws = null;
			}

			Console.WriteLine("--- закрытие книги Excel");
			if (wb != null) {
				wb.Close(false);
				Marshal.ReleaseComObject(wb);
				wb = null;
			}
		}

		public void CloseExcel() {
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
