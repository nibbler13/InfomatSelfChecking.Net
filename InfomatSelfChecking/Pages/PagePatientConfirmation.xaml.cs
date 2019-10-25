using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using InfomatSelfChecking.Items;

namespace InfomatSelfChecking.Pages {
    /// <summary>
    /// Логика взаимодействия для PagePatientSelectedSingle.xaml
    /// </summary>
    public partial class PagePatientConfirmation : Page {
		private readonly List<ItemPatient> patients;
		private ItemPatient selectedPatient;
		private readonly bool returnBack = false;
		private readonly string title;

        public PagePatientConfirmation(List<ItemPatient> patients) {
			InitializeComponent();
			Console.WriteLine(@"http://CONSTRUCT_PagePatientConfirmation");
			Logging.ToLog("PagePatientConfirmation - отображение страницы подтверждения ФИО пациента / выбора пациента");

			this.patients = patients ?? throw new ArgumentNullException(nameof(patients));

			if (patients.Count == 1) {
				TextBlockName.Text = patients[0].Name;
				TextBlockBirthday.Text = "Дата рождения: " + patients[0].Birthday.ToLongDateString();
				title = Properties.Resources.title_name_confirm;
			} else {
				GridSinglePatient.Visibility = Visibility.Hidden;
				GridMultiplePatients.Visibility = Visibility.Visible;
				title = Properties.Resources.title_name_confirm_multiple;
                DrawPatients();
				returnBack = true;
			}

			DataContext = BindingValues.Instance;
			IsVisibleChanged += (s, e) => {
				if ((bool)e.NewValue)
					BindingValues.Instance.SetUpMainWindow(title, true, false);
			};
		}

		~PagePatientConfirmation() {
			Console.WriteLine(@"http://DECONSTRUCT_PagePatientConfirmation");
		}

		private void DrawPatients() {
			int row = 0;

			foreach (ItemPatient patient in patients) {
                if (row > 1)
                    GridMultiplePatients.RowDefinitions.Add(new RowDefinition());

                Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };

                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                ColumnDefinition col0 = new ColumnDefinition { Width = new GridLength(90, GridUnitType.Star) };
                grid.ColumnDefinitions.Add(col0);

                ColumnDefinition col1 = new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) };
                grid.ColumnDefinitions.Add(col1);

                string textTop = patient.Name;
                string textBottom = "дата рождения: " + patient.Birthday.ToLongDateString();

                TextBlock textBlockTop = ControlsFactory.CreateTextBlock(textTop);
				textBlockTop.Foreground = BindingValues.Instance.BrushTextForeground;
				textBlockTop.Margin = new Thickness(20, 0, 0, 0);

                TextBlock textBlockBottom = ControlsFactory.CreateTextBlock(textBottom);
                textBlockBottom.Foreground = BindingValues.Instance.BrushTextDisabledForeground;
				textBlockBottom.Margin = new Thickness(20, 0, 0, 0);
                Grid.SetRow(textBlockBottom, 1);

                Image image = ControlsFactory.CreateImage(
                    ControlsFactory.ImageType.NotificationOk, 
                    horizontalAlignment: HorizontalAlignment.Right, 
                    margin: new Thickness(20));

                Grid.SetColumn(image, 1);
                Grid.SetRowSpan(image, 2);
                image.Visibility = Visibility.Hidden;

                grid.Children.Add(textBlockTop);
                grid.Children.Add(textBlockBottom);
                grid.Children.Add(image);

                Button button = new Button {
                    Tag = patient,
                    Style = BindingValues.Instance.StyleRoundCornerStretch,
                    Content = grid,
                    Margin = new Thickness(20)
                };

                button.Effect = ControlsFactory.CreateDropShadowEffect();
                button.Click += ButtonPatient_Click;

				Grid.SetRow(button, row);
				GridMultiplePatients.Children.Add(button);

                button.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                patient.CheckStateImage = image;
				
				row++;
			}
		}

		private void ButtonPatient_Click(object sender, RoutedEventArgs e) {
			ItemPatient itemPatient = (sender as Button).Tag as ItemPatient;
			CheckPatientStateAndShowAppointments(itemPatient);
		}

		private void ButtonWrong_Click(object sender, RoutedEventArgs e) {
			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.NameNotCorrect);
			NavigationService.Navigate(pageNotification);
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			CheckPatientStateAndShowAppointments(patients[0]);
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e) {
			MainWindow.Instance.CloseAllWindows();
		}

		private void ButtonYes_Click(object sender, RoutedEventArgs e) {
			PageShowAppointments pageShowAppointments;

			if (patients.Count == 1)
				pageShowAppointments = new PageShowAppointments(patients[0], false);
			else
				pageShowAppointments = new PageShowAppointments(selectedPatient, true);

			NavigationService.Navigate(pageShowAppointments);
		}

		private void ButtonNo_Click(object sender, RoutedEventArgs e) {
			PageNotification pageNotification = new PageNotification(
				PageNotification.NotificationType.AlreadyChecked, returnBack: patients.Count > 1);
			NavigationService.Navigate(pageNotification);
		}

		private void CheckPatientStateAndShowAppointments(ItemPatient patient) {
			PageNotification.NotificationType? notificationType = null;
			selectedPatient = patient;

			if (patient.StopCodesCurrent.Contains(ItemPatient.StopCode.FirstTime))
				notificationType = PageNotification.NotificationType.FirstVisit;
			//else if (patient.StopCodesCurrent.Contains(ItemPatient.StopCodes.NotAvailableNow))
			//	notificationType = PageNotification.NotificationType.NoAppointmentsForNow;
			else if (patient.StopCodesCurrent.Contains(ItemPatient.StopCode.Cash))
				notificationType = PageNotification.NotificationType.Cash;
			else if (patient.StopCodesCurrent.Count > 0)
				notificationType = PageNotification.NotificationType.VisitRegistryToCheckIn;

			if (notificationType.HasValue) {
				NavigationService.Navigate(new PageNotification(notificationType.Value, returnBack: returnBack));
				SetImage(patient, false);
				return;
			}

            bool isOk = true;

			if (patient.AppointmentsNotAvailable.Count >= 0 &&
				patient.AppointmentsAvailable.Count == 0 &&
				patient.AppointmentsVisited.Count == 0) {
				PageNotification pageAppointmentsNotFound =
					new PageNotification(PageNotification.NotificationType.NoAppointmentsForNow, returnBack: returnBack);
				NavigationService.Navigate(pageAppointmentsNotFound);
				isOk = false;
			} else if (
				patient.AppointmentsVisited.Count >= 0 &&
				patient.AppointmentsAvailable.Count == 0) {
				//do you want to see already checked appointments?
				BindingValues.Instance.SetUpMainWindow(patient.Name + ",", false, false);
				GridVisitedAppointmentsQuestion.Visibility = Visibility.Visible;

				if (patients.Count == 1) 
					GridSinglePatient.Visibility = Visibility.Hidden;
				else 
					GridMultiplePatients.Visibility = Visibility.Hidden;
            } else {
				PageShowAppointments pageAppointmentsShow = new PageShowAppointments(patient, returnBack);
				NavigationService.Navigate(pageAppointmentsShow);
			}

            SetImage(patient, isOk);
        }

		private static void SetImage(ItemPatient patient, bool isOk) {
			if (patient.CheckStateImage != null) {
				if (!isOk) 
					patient.CheckStateImage.Source = ControlsFactory.GetImage(ControlsFactory.ImageType.NotificationRegistry);

				patient.CheckStateImage.Visibility = Visibility.Visible;
			}
		}
	}
}
