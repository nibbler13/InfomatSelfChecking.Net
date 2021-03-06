﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace InfomatSelfChecking.Pages {
	/// <summary>
	/// Логика взаимодействия для PageEnterNumber.xaml
	/// </summary>
	public partial class PageEnterNumber : Page {
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

				BindingValues.Instance.UpdateDialerText(enteredNumber);
			}
		}

		public PageEnterNumber() {
			InitializeComponent();
			Console.WriteLine(@"http://CONSTRUCT_PageEnterNumber");

			Logging.ToLog("PageEnterNumber - отображение страницы набора номера");

			DataContext = BindingValues.Instance;
			BindingValues.Instance.SetUpMainWindow(Properties.Resources.title_dialer, true, false);
			BindingValues.Instance.InitializeDialerText();

			if (Debugger.IsAttached)
				EnteredNumber = "0000000000";
        }

		~PageEnterNumber() {
			Console.WriteLine(@"http://DECONSTRUCT_PageEnterNumber");
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
			if (EnteredNumber.Length < 10)
				return;

			Logging.ToLog("PageEnterNumber - введен номер: " + EnteredNumber);

            try {
                DataHandle.Instance.LoadPatients(EnteredNumber.Substring(0, 3), EnteredNumber.Substring(3, 7));
            } catch (Exception exc) {
                NavigationService.Navigate(new PageNotification(PageNotification.NotificationType.DbError, exception: exc));
                return;
            }

			Page page;

			if (DataHandle.PatientsCurrent.Count == 0) {
                string entered = "+7 (" + EnteredNumber.Substring(0, 3) +
                    ") " + EnteredNumber.Substring(3, 3) + "-" +
					EnteredNumber.Substring(6, 2) + "-" +
					EnteredNumber.Substring(8, 2);

                page = new PageNotification(PageNotification.NotificationType.NumberNotFound, entered);
            } else if (DataHandle.PatientsCurrent.Count > 4)
				page = new PageNotification(PageNotification.NotificationType.VisitRegistryToCheckIn);
			else
				page = new PagePatientConfirmation(DataHandle.PatientsCurrent);

			NavigationService.Navigate(page);
		}
	}
}
