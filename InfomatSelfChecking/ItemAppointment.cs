using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
    public class ItemAppointment {
		public string SchedID { get; set; } = string.Empty;
		public string DateTimeScheduleBegin { get; set; } = string.Empty;
		public string DateTimeScheduleEnd { get; set; } = string.Empty;
        public string DName { get; set; } = string.Empty;
		public string DepName { get; set; } = string.Empty;
		public string DepShortName { get; set; } = string.Empty;
		public string RNum { get; set; } = string.Empty;

        public int GetMinutesLeftToBegin() {
            DateTime dateTime = DateTime.Now.Date;
            string[] begins = DateTimeScheduleBegin.Split(':');
            dateTime = dateTime.AddHours(Convert.ToInt32(begins[0]));
            dateTime = dateTime.AddMinutes(Convert.ToInt32(begins[1]));
            return (int)(dateTime - DateTime.Now).TotalMinutes;
        }
	}
}
