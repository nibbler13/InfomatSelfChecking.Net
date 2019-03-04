using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfomatSelfChecking {
	class PageEnterNumberViewModel : BaseViewModel {


		private string dialerText;
		private string DialerText {
			get {
				return dialerText;
			}
			set {
				if (value != dialerText) {
					dialerText = value;
					NotifyPropertyChanged();
				}
			}
		}

		public String DialerText0 {
			get {
				return dialerText.Substring(0, 1);
			}
		}
		public String DialerText1 {
			get {
				return dialerText.Substring(1, 1);
			}
		}
		public String DialerText2 {
			get {
				return dialerText.Substring(2, 1);
			}
		}
		public String DialerText3 {
			get {
				return dialerText.Substring(3, 1);
			}
		}
		public String DialerText4 {
			get {
				return dialerText.Substring(4, 1);
			}
		}
		public String DialerText5 {
			get {
				return dialerText.Substring(5, 1);
			}
		}
		public String DialerText6 {
			get {
				return dialerText.Substring(6, 1);
			}
		}
		public String DialerText7 {
			get {
				return dialerText.Substring(7, 1);
			}
		}
		public String DialerText8 {
			get {
				return dialerText.Substring(8, 1);
			}
		}
		public String DialerText9 {
			get {
				return dialerText.Substring(9, 1);
			}
		}



		private string enteredNumber = string.Empty;
		private string EnteredNumber {
			get {
				return enteredNumber;
			}
			set {
				enteredNumber = value;

				//ButtonClear.IsEnabled = enteredNumber.Length > 0;
				//ButtonRemoveOne.IsEnabled = enteredNumber.Length > 0;
				//ButtonContinue.IsEnabled = enteredNumber.Length == 10;

				//for (int i = 0; i <= 9; i++) {
				//    string symbol = "_";
				//    if (i < enteredNumber.Length)
				//        symbol = enteredNumber.Substring(i, 1);

				//    TextBlockEntered[i].Text = symbol;
				//}
			}
		}



		public void InitializeDialerText() {
			DialerText = "__________";
		}

		public PageEnterNumberViewModel() {
			InitializeDialerText();

			MainViewModel.Instance.SetUpMainWindow(Properties.Resources.title_dialer, true, false);

			EnteredNumber = "0000000000";
		}



		//private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
		//	if (!(sender is Button button))
		//		return;

		//	if ((bool)e.NewValue == true) {
		//		button.Effect = ControlsFactory.CreateDropShadowEffect();

		//		if (button == ButtonContinue)
		//			button.Foreground = BindingValues.Instance.BrushTextTitleForeground;
		//		else
		//			button.Foreground = BindingValues.Instance.BrushTextForeground;

		//	} else {
		//		button.Effect = null;
		//		button.Foreground = BindingValues.Instance.BrushTextDisabledForeground;
		//	}
		//}

		//private void ButtonNumber_Click(object sender, RoutedEventArgs e) {
		//	if (EnteredNumber.Length == 10)
		//		return;

		//	EnteredNumber += ((Button)sender).Content;
		//}

		//private void ButtonClear_Click(object sender, RoutedEventArgs e) {
		//	EnteredNumber = string.Empty;
		//}

		//private void ButtonRemoveOne_Click(object sender, RoutedEventArgs e) {
		//	EnteredNumber = enteredNumber.Substring(0, enteredNumber.Length - 1);
		//}

		//private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
		//	if (enteredNumber.Length < 10)
		//		return;

		//	List<ItemPatient> patients;
		//	try {
		//		patients = DataHandle.GetPatients(enteredNumber.Substring(0, 3), enteredNumber.Substring(3, 7));
		//	} catch (Exception exc) {
		//		NavigationService.Navigate(new PageNotification(PageNotification.NotificationType.DbError, exception: exc));
		//		return;
		//	}

		//	Page page;

		//	if (patients.Count == 0) {
		//		string entered = "+7 (" + enteredNumber.Substring(0, 3) +
		//			") " + enteredNumber.Substring(3, 3) + "-" +
		//			enteredNumber.Substring(5, 2) + "-" +
		//			enteredNumber.Substring(7, 2);

		//		page = new PageNotification(PageNotification.NotificationType.NumberNotFound, entered);
		//	} else if (patients.Count > 4)
		//		page = new PageNotification(PageNotification.NotificationType.TooManyPatients);
		//	else
		//		page = new PagePatientConfirmation(patients);

		//	NavigationService.Navigate(page);
		//}
	}
}
