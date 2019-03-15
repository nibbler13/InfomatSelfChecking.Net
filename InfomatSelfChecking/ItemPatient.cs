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
            Debt
		}

		public enum InfoCodes {
			InformAboutLK
		}

		public string PhoneNumber { get; set; } = string.Empty;
		public string PCode { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public DateTime Birthday { get; set; }

		public List<StopCodes> StopCodesCurrent { get; set; } = new List<StopCodes>();
		public List<InfoCodes> InfoCodesCurrent { get; set; } = new List<InfoCodes>();

		public List<ItemAppointment> AppointmentsVisited { get; set; } = new List<ItemAppointment>();
		public List<ItemAppointment> AppointmentsAvailable { get; set; } = new List<ItemAppointment>();
		public List<ItemAppointment> AppointmentsNotAvailable { get; set; } = new List<ItemAppointment>();
		
		public Image CheckStateImage { get; set; } = null;

		private Excel.Worksheet worksheet = null;
		private Excel.Workbook workbook = null;
		private PrinterInfo.State? printerState = null;
		//private BackgroundWorker backgroundWorker = null;
		public ManualResetEvent BackgroundWorkCompletedEvent { get; private set; } = new ManualResetEvent(false);
		public bool IsWorksheetCreated { get; private set; } = false;
		private bool IsWorksheetCreatingStarted = false;

		public async void CheckPrinterAndCreateWorksheet() {
			if (IsWorksheetCreated)
				return;

			IsWorksheetCreatingStarted = true;

			await Task.Run(() => {
				printerState = PrinterInfo.GetPrinterState();

				if (printerState == PrinterInfo.State.Ready)
					worksheet = ExcelInterop.Instance.CreateWorksheetAppointmentsAvailable(this, out workbook);

				IsWorksheetCreated = true;
				BackgroundWorkCompletedEvent.Set();
			});
		}

		public PrinterInfo.State PrintAppointmentsAvailable() {
			Console.WriteLine("--- Печать назначений пациента");
			if (!IsWorksheetCreatingStarted)
				CheckPrinterAndCreateWorksheet();

			if (!IsWorksheetCreated)
				BackgroundWorkCompletedEvent.WaitOne();

				Console.WriteLine("=== Проверка состояния принтера");
				if (printerState.HasValue) {
					switch (printerState.Value) {
						case PrinterInfo.State.NotSelect:
						case PrinterInfo.State.NotFound:
						case PrinterInfo.State.Error:
							Console.WriteLine("=== возврат полученного состояния");
							return printerState.Value;
					}

					if (worksheet == null) {
						Console.WriteLine("=== возврат состояние не напечатано");
						return PrinterInfo.State.NotPrinted;
					}

					ExcelInterop.Instance.PrintWorksheetAndCloseWorkbook(ref worksheet, ref workbook, Name + "_" + PhoneNumber);

					Console.WriteLine("=== возврат состояния напечатано");
					return PrinterInfo.State.Printed;
				}

				Console.WriteLine("=== возврат состояние неизвестно 1");
				return PrinterInfo.State.Unknown;
		}

		//public void CancelCreateWorksheet() {
		//	if (backgroundWorker == null)
		//		return;

		//	if (!backgroundWorker.IsBusy)
		//		return;

		//	backgroundWorker.CancelAsync();
		//}
	}
}
