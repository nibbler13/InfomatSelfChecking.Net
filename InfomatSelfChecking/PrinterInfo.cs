using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
	public class PrinterInfo {
		private static readonly string printerName = Properties.Settings.Default.PrinterName;
		private static readonly string mailReceiver = Properties.Settings.Default.MailTo;
		private static readonly string nameSpace = @"\root\CIMV2";
		private static readonly string className = "Win32_Printer";
		private static readonly Dictionary<int, string> statusCodes = new Dictionary<int, string> {
			{ 0,       "Printer ready" },
			{ 1,       "Printer paused" },
			{ 2,       "Printer error" },
			{ 4,       "Printer pending deletion" },
			{ 8,       "Paper jam" },
			{ 16,      "Out of paper" },
			{ 32,      "Manual feed" },
			{ 64,      "Paper problem" },
			{ 128,     "Printer offline" },
			{ 256,     "IO active" },
			{ 512,     "Printer busy" },
			{ 1024,    "Printing" },
			{ 2048,    "Printer output bin full" },
			{ 4096,    "Not available." },
			{ 8192,    "Waiting" },
			{ 16384,   "Processing" },
			{ 32768,   "Initializing" },
			{ 65536,   "Warming up" },
			{ 131072,  "Toner low" },
			{ 262144,  "No toner" },
			{ 524288,  "Page punt" },
			{ 1048576, "User intervention" },
			{ 2097152, "Out of memory" },
			{ 4194304, "Door open" },
			{ 8388608, "Server unknown" },
			{ 6777216, "Power save" }
		};

		public enum State {
			Unknown,
			NotSelect,
			NotFound,
			Ready,
			Error,
			Printed,
			NotPrinted
		}
		
		public static State GetPrinterState() {
			if (string.IsNullOrEmpty(printerName))
				return State.NotSelect;

			try {
				ManagementObjectSearcher mgmtObjSearcher = new ManagementObjectSearcher(nameSpace, "SELECT * FROM " + className);
				ManagementObjectCollection colPrinters = mgmtObjSearcher.Get();

				if (colPrinters.Count == 0) {
					Logging.ToLog("Не удалось получить информацию об установленных принтеров в разделе " + 
						nameSpace + ", " + className);
					return State.NotFound;
				}

				foreach (ManagementObject printer in colPrinters) {
					string currentPrinterName = printer["Name"].ToString();

					if (!currentPrinterName.Equals(printerName))
						continue;

					long printerState = Convert.ToInt64(printer["PrinterState"]);
					bool printerWorkOffline = Convert.ToBoolean(printer["WorkOffline"]);

					if ((printerState == 0 || //"Printer ready"
						printerState == 131072 || //"Toner low"
						printerState == 131072 + 2048) && //"Toner low" + "Printer output bin full"
						!printerWorkOffline)
						return State.Ready;

					string printerStatus = string.Empty;

					if (printerWorkOffline)
						printerStatus += "Printer is working offline" + Environment.NewLine;

					for (int i = statusCodes.Count - 1; i >= 0; i--) {
						if (printerState == 0)
							break;

						int code = statusCodes.ElementAt(i).Key;
						
						if ((printerState - code) < 0)
							continue;

						printerState -= code;

						if (code == 131073 || code == 2048) //"Toner low" || "Printer output bin full"
							continue;

						printerStatus += statusCodes[code] + Environment.NewLine;
					}

					if (!string.IsNullOrEmpty(printerStatus)) {
						string subject = "Уведомление от инфомата";
						string body = "Инфомату не удалось распечатать список назначений пациента, коды ошибок принтера: " +
							Environment.NewLine + Environment.NewLine + printerStatus;
						Mail.SendMail(subject, body, mailReceiver);
					}

					return State.Error;
				}
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			return State.Error;
		}
	}
}
