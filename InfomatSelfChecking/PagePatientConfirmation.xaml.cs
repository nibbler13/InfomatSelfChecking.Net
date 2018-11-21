using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для PagePatientSelectedSingle.xaml
    /// </summary>
    public partial class PagePatientConfirmation : Page {
		private List<ItemPatient> patients;
		private bool returnBack = false;

        public PagePatientConfirmation(List<ItemPatient> patients) {
			InitializeComponent();

			this.patients = patients;
			bool isLogoVisible = false;

			string title;
			if (patients.Count == 1) {
				TextBlockName.Text = patients[0].Name;
				TextBlockBirthday.Text = "Дата рождения: " + patients[0].Birthday;
				title = Properties.Resources.title_name_confirm;
			} else {
				GridSinglePatient.Visibility = Visibility.Hidden;
				GridMultiplePatients.Visibility = Visibility.Visible;
				ButtonWrong.Content = "Закрыть";
				Grid.SetColumn(ButtonWrong, 1);
				ButtonContinue.Visibility = Visibility.Hidden;
				title = Properties.Resources.title_name_confirm_multiple;
				KeepAlive = true;
				DrawPatients();
				isLogoVisible = true;
				returnBack = true;
			}

			ButtonWrong.Style = Application.Current.MainWindow.FindResource("RoundCorner") as Style;
			ButtonContinue.Style = Application.Current.MainWindow.FindResource("RoundCornerGreen") as Style;

			MainWindow.ConfigurePage(this);

			TextBlockName.FontSize = FontSize * 1.3;
			TextBlockBirthday.FontSize = FontSize * 1.3;

			Loaded += (s, e) => {
				MainWindow.AppMainWindow.SetUpWindow(isLogoVisible, title, false);
			};
        }

		private void DrawPatients() {
			int row = 0;

			foreach (ItemPatient patient in patients) {
				if (row > 1)
					GridMultiplePatients.RowDefinitions.Add(new RowDefinition());

				string text = patient.Name + Environment.NewLine +
						"Дата рождения: " + patient.Birthday;
				TextBlock textBlock = ControlsFactory.CreateTextBlock(text, margin: new Thickness(10));
				Button button = ControlsFactory.CreateButton(textBlock, margin: new Thickness(20), tag: patient);
				button.Click += ButtonPatient_Click;
				Grid.SetRow(button, row);
				Grid.SetColumnSpan(button, 3);
				GridMultiplePatients.Children.Add(button);

				Image image = ControlsFactory.CreateImage(ControlsFactory.ImageType.NotificationOk, margin: new Thickness(0, 20, 40, 20));
				image.Visibility = Visibility.Hidden;
				Grid.SetRow(image, row);
				Grid.SetColumn(image, 3);
				GridMultiplePatients.Children.Add(image);
				patient.CheckStateImage = image;
				
				row++;
			}
		}

		private void ButtonPatient_Click(object sender, RoutedEventArgs e) {
			ItemPatient itemPatient = (sender as Button).Tag as ItemPatient;
			CheckPatientStateAndShowAppointments(itemPatient);
		}

		private void ButtonWrong_Click(object sender, RoutedEventArgs e) {
			if (patients.Count == 1) {
				PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.NameNotCorrect);
				NavigationService.Navigate(pageNotification);
			} else {
				MainWindow.AppMainWindow.CloseAllWindows();
			}
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			CheckPatientStateAndShowAppointments(patients[0]);
		}

		private void CheckPatientStateAndShowAppointments(ItemPatient patient) {
			Page pageError = null;

			if (patient.IsCardBlocked)
				pageError = new PageNotification(PageNotification.NotificationType.CardBlocked, returnBack: returnBack);

			if (patient.IsFirstVisit)
				pageError = new PageNotification(PageNotification.NotificationType.FirstVisit, returnBack: returnBack);

			if (pageError != null) {
				NavigationService.Navigate(pageError);
				SetImage(patient, false);
				return;
			}

			DataHandle.UpdatePatientAppointments(ref patient);
			bool isOk;

			if (patient.Appointments.Count == 0) {
				PageNotification pageAppointmentsNotFound = 
					new PageNotification(PageNotification.NotificationType.NoAppointmentsForNow, returnBack: returnBack);
				NavigationService.Navigate(pageAppointmentsNotFound);
				isOk = false;
			} else {
				bool isAnyCash = patient.Appointments.Where(e => e.IsCash).Count() > 0;
				bool isLate = patient.Appointments.Where(e => e.IsLate).Count() > 0;
				bool isAnyRoentgen = patient.Appointments.Where(e => e.IsRoentgen).Count() > 0;
				isOk = !isAnyCash && !isLate && !isAnyRoentgen;

				PageShowAppointments pageAppointmentsShow = new PageShowAppointments(patient, returnBack);
				NavigationService.Navigate(pageAppointmentsShow);
			}

			SetImage(patient, isOk);
		}

		private void SetImage(ItemPatient patient, bool isOk) {
			if (patient.CheckStateImage != null) {
				if (!isOk) 
					patient.CheckStateImage.Source = ControlsFactory.GetImage(ControlsFactory.ImageType.NotificationRegistry);

				patient.CheckStateImage.Visibility = Visibility.Visible;
			}
		}
	}
}
