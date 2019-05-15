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
		private readonly string stpAddress = Properties.Settings.Default.MailSTP;
		private readonly string subject = "Ошибка в работе инфомата";
		private readonly string templateFileName = "PrintTemplate.xlsx";

		private const int ROW_CLINIC_ADDRESS = 2;
		private const int ROW_CLINIC_PHONE_NUMBER = 3;
		private const int ROW_DATE_TIME = 4;
		private const int ROW_NAME = 5;
		private const int ROW_FAMILY = 6;
		private const int ROW_STYLE = 7;
		private const int ROW_DELIMITER = 8;
		private const int ROW_START = 9;

		public ExcelInterop() {
			Logging.ToLog("ExcelInterop - Запуск Excel");
			xlApp = new Excel.Application();
			if (xlApp == null) {
				string msg = "ExcelInterop - Не удалось открыть приложение Excel";
				Logging.ToLog(msg);
				Mail.SendMail(subject, msg, stpAddress);
				return;
			}

			xlApp.Visible = false;
			string currentDir = Directory.GetCurrentDirectory();
			templateFullPath = Path.Combine(currentDir, "ExcelTemplate", templateFileName);
			if (!File.Exists(templateFullPath)) {
				string msg = "ExcelInterop - Не удалось получить доступ к файлу шаблона: " + templateFullPath;
				Logging.ToLog(msg);
				Mail.SendMail(subject, msg, stpAddress);
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

			DeleteOldFiles();
		}

		private void DeleteOldFiles() {
			Logging.ToLog("ExcelInterop - Удаление старых файлов Excel");

			try {
				DirectoryInfo dirInfo = new DirectoryInfo(saveFolder);
				FileInfo[] files = dirInfo.GetFiles("*.xlsx");

				for (int i = 0; i < files.Length; i++) {
					FileInfo info = files[i];

					if ((DateTime.Now - info.CreationTime).TotalDays < 7)
						continue;

					files[i].Delete();
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
			}
		}

		private Excel.Worksheet OpenTemplate(out Excel.Workbook wb, string filePostfix) {
			try {
				Logging.ToLog("ExcelInterop - Открытие книги: " + templateFullPath);

				//CheckIfTemplateIsOpened();

				string currentTemplate = Path.Combine(saveFolder, "PrintResult_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + filePostfix + ".xlsx");
				File.Copy(templateFullPath, currentTemplate);

				wb = xlApp.Workbooks.Open(currentTemplate);

				if (wb == null) {
					string msg = "ExcelInterop - Не удалось открыть книгу: " + templateFullPath;
					Logging.ToLog(msg);
					Mail.SendMail(subject, msg, stpAddress);
					return null;
				}

				Logging.ToLog("ExcelInterop - Открытие листа: Template");
				Excel.Worksheet ws = wb.Sheets["Template"];
				if (ws == null) {
					string msg = "ExcelInterop - Не удалось открыть лист: Template";
					Logging.ToLog(msg);
					Mail.SendMail(subject, msg, stpAddress);
					return null;
				}

				return ws;
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			wb = null;
			return null;
		}

		//private void CheckIfTemplateIsOpened() {
		//	try {
		//		Excel.Workbook wb = xlApp.Workbooks.get_Item(templateFileName);
		//		wb.Close(false);
		//	} catch (Exception) { }
		//}

		public Excel.Worksheet CreateWorksheetAppointmentsAvailable(ItemPatient patient, out Excel.Workbook wb) {
			Logging.ToLog("ExcelInterop - Печать назначений для пациента: " + patient.Name + ", pcode: " + patient.PCode);
			wb = null;

			if (xlApp == null) {
				Logging.ToLog("ExcelInterop - Не запущен Excel, пропуск");
				return null;
			}

			Excel.Worksheet ws = OpenTemplate(out wb, patient.Name + "_" + patient.PhoneNumber);
			if (ws == null)
				return null;

			Logging.ToLog("ExcelInterop - Запись информации о назначениях");
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

		public void PrintWorksheetAndCloseWorkbook(ref Excel.Worksheet ws, ref Excel.Workbook wb) {
			ws.PrintOutEx();
			CloseWorkbook(ref ws, ref wb);
		}

		private void CloseWorkbook(ref Excel.Worksheet ws, ref Excel.Workbook wb) {
			if (!string.IsNullOrEmpty(saveFolder))
				wb.Save();

			if (ws != null) {
				Marshal.ReleaseComObject(ws);
				ws = null;
			}

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
