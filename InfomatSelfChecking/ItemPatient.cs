using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace InfomatSelfChecking {
	public class ItemPatient {
		public enum StopCodes {
			Cash,
			FirstTime,
			Lock,
			Late,
			NotAvailableNow,
			DepOut,
            Debt,
			Agreement
		}

		public enum InfoCodes {
			InformAboutLK
		}

		public string PhoneNumber { get; set; } = string.Empty;
		public string PCode { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public DateTime Birthday { get; set; }

		public List<StopCodes> StopCodesCurrent { get; } = new List<StopCodes>();
		public List<InfoCodes> InfoCodesCurrent { get; } = new List<InfoCodes>();

		public List<ItemAppointment> AppointmentsVisited { get; } = new List<ItemAppointment>();
		public List<ItemAppointment> AppointmentsAvailable { get; } = new List<ItemAppointment>();
		public List<ItemAppointment> AppointmentsNotAvailable { get; } = new List<ItemAppointment>();
		
		public Image CheckStateImage { get; set; } = null;

		private Excel.Worksheet worksheet = null;
		private Excel.Workbook workbook = null;
		private PrinterInfo.State? printerState = null;
		public ManualResetEvent BackgroundWorkCompletedEvent { get; private set; } = new ManualResetEvent(false);
		public bool IsWorksheetCreated { get; private set; } = false;
		private bool IsWorksheetCreatingStarted = false;

		public async void CheckPrinterAndCreateWorksheet() {
			if (IsWorksheetCreated)
				return;

			Logging.ToLog("ItemPatient - создание книги со списком назначений");

			IsWorksheetCreatingStarted = true;

			await Task.Run(() => {
				printerState = PrinterInfo.Instance.GetPrinterState();
				Logging.ToLog("ItemPatient - Полученный статус принтера: " + printerState);

				try {
					if (printerState == PrinterInfo.State.DoNotCheck ||
						printerState == PrinterInfo.State.Ready)
						worksheet = ExcelInterop.Instance.CreateWorksheetAppointmentsAvailable(this, out workbook);
				} catch (Exception e) {
					printerState = PrinterInfo.State.NotPrinted;
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
				}

				IsWorksheetCreated = true;
				BackgroundWorkCompletedEvent.Set();
			}).ConfigureAwait(false);
		}

		public PrinterInfo.State PrintAppointmentsAvailable() {
			Logging.ToLog("ItemPatient - печать списка назначений");

			if (!IsWorksheetCreatingStarted)
				CheckPrinterAndCreateWorksheet();

			if (!IsWorksheetCreated)
				BackgroundWorkCompletedEvent.WaitOne();

			if (printerState.HasValue) {
				switch (printerState.Value) {
					case PrinterInfo.State.NotSelect:
					case PrinterInfo.State.NotFound:
					case PrinterInfo.State.Error:
						return printerState.Value;
				}

				if (worksheet == null)
					return PrinterInfo.State.NotPrinted;

				try {
					ExcelInterop.Instance.PrintWorksheetAndCloseWorkbook(ref worksheet, ref workbook);
					return PrinterInfo.State.Printed;
				} catch (Exception e) {
					Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
					return PrinterInfo.State.NotPrinted;
				}
			}

			return PrinterInfo.State.Unknown;
		}

		public void CloseExcelWorkbook() {
			ExcelInterop.Instance.CloseWorkbook(ref worksheet, ref workbook);
		}

		public override string ToString() {
			string result = Environment.NewLine + "-----ItemPatient-----" + Environment.NewLine;

			result += "PhoneNumber: " + PhoneNumber + Environment.NewLine;
			result += "Name: " + Name + Environment.NewLine;
			result += "PCODE: " + PCode + Environment.NewLine;
			result += "Birthday: " + Birthday + Environment.NewLine;
			result += "StopCodes: " + string.Join(", ", StopCodesCurrent) + Environment.NewLine;
			result += "InfoCodes: " + string.Join(", ", InfoCodesCurrent) + Environment.NewLine;
			result += "AvailableAppointments: " + string.Join(Environment.NewLine, AppointmentsAvailable) + Environment.NewLine;
			result += "NotAvailableAppointments: " + string.Join(Environment.NewLine, AppointmentsNotAvailable) + Environment.NewLine;
			result += "VisitedAppointments: " + string.Join(Environment.NewLine, AppointmentsVisited) + Environment.NewLine;
			result += "=====End ItemPatient=====";

			return result;
		}
	}
}
