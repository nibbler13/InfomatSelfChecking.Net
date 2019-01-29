using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfomatSelfChecking {
	/// <summary>
	/// Логика взаимодействия для PageEnterNumber.xaml
	/// </summary>
	public partial class PageEnterNumber : Page {
        private readonly List<TextBlock> TextBlockEntered = new List<TextBlock>();
		private string enteredNumber = string.Empty;
		private string EnteredNumber {
			get {
				return enteredNumber;
			}
			set {
				enteredNumber = value;

				ButtonClear.IsEnabled = enteredNumber.Length > 0;
				ButtonRemoveOne.IsEnabled = enteredNumber.Length > 0;
				ButtonContinue.IsEnabled = enteredNumber.Length == 10;

                //for (int i = 0; i <= 9; i++) {
                //    string symbol = "_";
                //    if (i < enteredNumber.Length)
                //        symbol = enteredNumber.Substring(i, 1);

                //    TextBlockEntered[i].Text = symbol;
                //}
			}
		}
		


		public PageEnterNumber() {
			InitializeComponent();
			
			BindingValues.Instance.SetUpMainWindow(Properties.Resources.title_dialer, true, false);
			BindingValues.Instance.InitializeDialerText();

            //TextBlockEntered = new List<TextBlock>() {
            //    TextBlockNum1,
            //    TextBlockNum2,
            //    TextBlockNum3,
            //    TextBlockNum4,
            //    TextBlockNum5,
            //    TextBlockNum6,
            //    TextBlockNum7,
            //    TextBlockNum8,
            //    TextBlockNum9,
            //    TextBlockNum10,
            //};

            EnteredNumber = "0000000000";

			DataContext = BindingValues.Instance;
        }

		private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (!(sender is Button button))
				return;

			if ((bool)e.NewValue == true) {
				button.Effect = ControlsFactory.CreateDropShadowEffect();

				if (button == ButtonContinue)
					button.Foreground = BindingValues.Instance.BrushTextTitleForeground;
				else
					button.Foreground = BindingValues.Instance.BrushTextForeground;

			} else {
				button.Effect = null;
				button.Foreground = BindingValues.Instance.BrushTextDisabledForeground;
			}
		}

		private void ButtonNumber_Click(object sender, RoutedEventArgs e) {
			if (EnteredNumber.Length == 10)
				return;

			EnteredNumber += ((Button)sender).Content;
		}

		private void ButtonClear_Click(object sender, RoutedEventArgs e) {
			EnteredNumber = string.Empty;
		}

		private void ButtonRemoveOne_Click(object sender, RoutedEventArgs e) {
			EnteredNumber = enteredNumber.Substring(0, enteredNumber.Length - 1);
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			if (enteredNumber.Length < 10)
				return;

            List<ItemPatient> patients;
            try {
                patients = DataHandle.GetPatients(enteredNumber.Substring(0, 3), enteredNumber.Substring(3, 7));
            } catch (Exception exc) {
                NavigationService.Navigate(new PageNotification(PageNotification.NotificationType.DbError, exception: exc));
                return;
            }

			Page page;

			if (patients.Count == 0) {
                string entered = "+7 (" + enteredNumber.Substring(0, 3) +
                    ") " + enteredNumber.Substring(3, 3) + "-" +
                    enteredNumber.Substring(5, 2) + "-" +
                    enteredNumber.Substring(7, 2);

                page = new PageNotification(PageNotification.NotificationType.NumberNotFound, entered);
            }
			else if (patients.Count > 4)
				page = new PageNotification(PageNotification.NotificationType.TooManyPatients);
			else
				page = new PagePatientConfirmation(patients);

			NavigationService.Navigate(page);
		}
	}
}
