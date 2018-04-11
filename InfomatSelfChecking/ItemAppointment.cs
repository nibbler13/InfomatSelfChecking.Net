using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
    public class ItemAppointment {
		public string SchedID { get; set; } = string.Empty;
		public DateTime DateTimeBegin { get; set; } = new DateTime();
		public string DName { get; set; } = string.Empty;
		public string DepName { get; set; } = string.Empty;
		public string DepShortName { get; set; } = string.Empty;
		public string RNum { get; set; } = string.Empty;
		public bool IsLate { get; set; } = false;
		public bool IsCash { get; set; } = false;
		public bool IsRoentgen { get; set; } = false;
		public string ClVisit { get; set; } = string.Empty;
	}
}
