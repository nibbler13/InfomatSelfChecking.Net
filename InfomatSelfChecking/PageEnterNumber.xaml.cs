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
		private string phoneMask = "+7 (___) ___-__-__";
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

				string enteredPhone = phoneMask;
				Regex regex = new Regex(Regex.Escape("_"));
				foreach (char symbol in enteredNumber)
					enteredPhone = regex.Replace(enteredPhone, symbol.ToString(), 1);

				TextBlockEntered.Text = enteredPhone;
			}
		}




		public PageEnterNumber() {
			InitializeComponent();

			EnteredNumber = "9601811873";
			ButtonContinue.Background = MainWindow.BrushButtonOkBackground;
			ButtonContinue.Foreground = MainWindow.BrushTextHeaderForeground;

			foreach (Control item in GridNumbers.Children)
				item.Effect = ControlsFactory.CreateDropShadowEffect();

			MainWindow.ConfigurePage(this);
			MainWindow.AppMainWindow.SetUpWindows(true, Properties.Resources.title_dialer, false);
			TextBlockEntered.FontSize = FontSize * 2;

			ButtonClear.IsEnabledChanged += Button_IsEnabledChanged;
			ButtonRemoveOne.IsEnabledChanged += Button_IsEnabledChanged;
			ButtonContinue.IsEnabledChanged += Button_IsEnabledChanged;
		}

		private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Button button = sender as Button;
			if (button == null)
				return;

			if ((bool)e.NewValue == true) {
				button.Effect = ControlsFactory.CreateDropShadowEffect();
				if (button == ButtonContinue) {
					button.Foreground = MainWindow.BrushTextHeaderForeground;
				} else {
					button.Foreground = MainWindow.BrushTextForeground;
				}

			} else {
				button.Effect = null;
				button.Foreground = MainWindow.BrushTextDisabledForeground;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
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

			List<ItemPatient> patients = DataHandle.GetPatients(enteredNumber.Substring(0, 3), enteredNumber.Substring(3, 7));
			Page page = null;

			if (patients.Count == 0) {
				page = new PageNotification(PageNotification.NotificationType.NumberNotFound, TextBlockEntered.Text);
			} else if (patients.Count > 4) {
				page = new PageNotification(PageNotification.NotificationType.TooManyPatients);
			} else {
				page = new PagePatientConfirmation(patients);
			}

			NavigationService.Navigate(page);
		}
	}
}
