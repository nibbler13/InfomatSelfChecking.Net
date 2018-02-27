using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
    public class ItemPatient {
		public string PhoneNumber { get; set; } = string.Empty;
		public string PCode { get; set; } = string.Empty;
		public string Firstname { get; set; } = string.Empty;
		public string MiddleName { get; set; } = string.Empty;
		public string Birthday { get; set; } = string.Empty;
		public string IsFirstVisit { get; set; } = string.Empty;
		public string IsCardBlocked { get; set; } = string.Empty;
		public string HasOnlineAccount { get; set; } = string.Empty;
		public string Sex { get; set; } = string.Empty;
		public List<ItemAppointment> Appointments { get; set; } = new List<ItemAppointment>();
	}
}
