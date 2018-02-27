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

				TextBoxEntered.Text = enteredPhone;
			}
		}

		public PageEnterNumber() {
			InitializeComponent();

			EnteredNumber = "9601811873";
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
			if (patients.Count == 0) {
				PagePatientNotFound pagePatientNotFound = new PagePatientNotFound();
				NavigationService.Navigate(pagePatientNotFound);
			} else if (patients.Count == 1) {
				PagePatientSelectedSingle pagePatientSelectedSingle = new PagePatientSelectedSingle(patients.First());
				NavigationService.Navigate(pagePatientSelectedSingle);
			}
		}
	}
}
