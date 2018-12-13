using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InfomatSelfChecking {
    public class ItemPatient {
		public enum StopCodes {
			Cash,
			FirstTime,
			Lock,
			Late,
			NotAvailableNow,
			DepOut
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
	}
}
