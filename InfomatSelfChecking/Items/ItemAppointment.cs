using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking.Items {
    public class ItemAppointment {
		public string SchedID { get; set; } = string.Empty;
		public string DateTimeScheduleBegin { get; set; } = string.Empty;
		public string DateTimeScheduleEnd { get; set; } = string.Empty;
        public string DName { get; set; } = string.Empty;
		public string DepName { get; set; } = string.Empty;
		public string DepShortName { get; set; } = string.Empty;
		public string RNum { get; set; } = string.Empty;
		public bool AlreadyChecked { get; set; } = false;

        public int GetMinutesLeftToBegin() {
            DateTime dateTime = DateTime.Now.Date;
            string[] begins = DateTimeScheduleBegin.Split(':');
            dateTime = dateTime.AddHours(Convert.ToInt32(begins[0]));
            dateTime = dateTime.AddMinutes(Convert.ToInt32(begins[1]));
            return (int)(dateTime - DateTime.Now).TotalMinutes;
        }

		public override string ToString() {
			string result = Environment.NewLine + "-----ItemAppointment-----" + Environment.NewLine;

			result += "SchedID: " + SchedID + Environment.NewLine;
			result += "Interval: " + DateTimeScheduleBegin + " - " + DateTimeScheduleEnd + Environment.NewLine;
			result += "DoctorName: " + DName + Environment.NewLine;
			result += "DepartmentName: " + DepName + Environment.NewLine;
			result += "Room: " + RNum + Environment.NewLine;
			result += "AlreadyChecked: " + AlreadyChecked + Environment.NewLine;
			result += "=====End ItemAppointment=====";

			return result;
		}
	}
}
