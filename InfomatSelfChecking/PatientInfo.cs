using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
	class PatientInfo {
		private static PatientInfo instance = null;
		private static readonly object padlock = new object();

		public static PatientInfo Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new PatientInfo();

					return instance;
				}
			}
		}
	}
}
