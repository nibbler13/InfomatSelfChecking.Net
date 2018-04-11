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
		public string FirstName { get; set; } = string.Empty;
		public string MiddleName { get; set; } = string.Empty;
		public string Birthday { get; set; } = string.Empty;
		public bool IsFirstVisit { get; set; } = false;
		public bool IsCardBlocked { get; set; } = false;
		public bool HasOnlineAccount { get; set; } = false;
		public int Sex { get; set; } = -1;
		public List<ItemAppointment> Appointments { get; set; } = new List<ItemAppointment>();
		public Image image = null;
	}
}
