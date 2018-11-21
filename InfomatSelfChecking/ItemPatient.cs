using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InfomatSelfChecking {
    public class ItemPatient {
		public string PhoneNumber { get; set; } = string.Empty;
		public string PCode { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Birthday { get; set; } = string.Empty;
		public bool IsFirstVisit { get; set; } = false;
		public bool IsCardBlocked { get; set; } = false;
		public bool HasOnlineAccount { get; set; } = false;
		public List<ItemAppointment> Appointments { get; set; } = new List<ItemAppointment>();
		public Image CheckStateImage { get; set; } = null;
	}
}
