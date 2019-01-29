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
		private readonly List<ItemPatient> patients;
		private readonly bool returnBack = false;

        public PagePatientConfirmation(List<ItemPatient> patients) {
			InitializeComponent();

			this.patients = patients;
			bool isLogoVisible = false;

			string title;
			if (patients.Count == 1) {
				TextBlockName.Text = patients[0].Name;
				TextBlockBirthday.Text = "Дата рождения: " + patients[0].Birthday.ToLongDateString();
				title = Properties.Resources.title_name_confirm;
			} else {
				GridSinglePatient.Visibility = Visibility.Hidden;
				GridMultiplePatients.Visibility = Visibility.Visible;
				TextBlockButtonWrong.Text = "Закрыть";
                Grid.SetColumn(ButtonWrong, 1);
                ButtonContinue.Visibility = Visibility.Hidden;
				title = Properties.Resources.title_name_confirm_multiple;
				KeepAlive = true;
                DrawPatients();
                isLogoVisible = true;
				returnBack = true;
			}

			ButtonWrong.Style = Application.Current.MainWindow.FindResource("RoundCorner") as Style;
            TextBlockButtonWrong.Foreground = Properties.Settings.Default.BrushTextForeground;
			ButtonContinue.Style = Application.Current.MainWindow.FindResource("RoundCornerGreen") as Style;

			//MainWindow.ConfigurePage(this);

			TextBlockName.FontSize = FontSize * 1.2;
			TextBlockBirthday.FontSize = FontSize * 1.2;

			//Loaded += (s, e) => {
			//	MainWindow.CurrentMainWindow.SetUpMainWindow(isLogoVisible, title, false);
			//};
        }

		private void DrawPatients() {
			int row = 0;

			foreach (ItemPatient patient in patients) {
                if (row > 1)
                    GridMultiplePatients.RowDefinitions.Add(new RowDefinition());

                Grid grid = new Grid {
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                ColumnDefinition col0 = new ColumnDefinition {
                    Width = new GridLength(10, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(col0);

                ColumnDefinition col1 = new ColumnDefinition {
                    Width = new GridLength(80, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(col1);

                ColumnDefinition col2 = new ColumnDefinition {
                    Width = new GridLength(10, GridUnitType.Star)
                };
                grid.ColumnDefinitions.Add(col2);

                string textTop = patient.Name;
                string textBottom = "дата рождения: " + patient.Birthday.ToLongDateString();

                TextBlock textBlockTop = ControlsFactory.CreateTextBlock(textTop);
                textBlockTop.Foreground = Properties.Settings.Default.BrushTextForeground;
                Grid.SetColumn(textBlockTop, 1);

                TextBlock textBlockBottom = ControlsFactory.CreateTextBlock(textBottom);
                textBlockBottom.Foreground = Properties.Settings.Default.BrushTextDisabledForeground;
                Grid.SetRow(textBlockBottom, 1);
                Grid.SetColumn(textBlockBottom, 1);

                Image image = ControlsFactory.CreateImage(
                    ControlsFactory.ImageType.NotificationOk, 
                    horizontalAlignment: HorizontalAlignment.Right, 
                    margin: new Thickness(20));

                Grid.SetColumn(image, 2);
                Grid.SetRowSpan(image, 2);
                image.Visibility = Visibility.Hidden;

                grid.Children.Add(textBlockTop);
                grid.Children.Add(textBlockBottom);
                grid.Children.Add(image);

                Button button = new Button {
                    Tag = patient,
                    Style = Application.Current.MainWindow.FindResource("RoundCornerStretch") as Style,
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
			if (patients.Count == 1) {
				PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.NameNotCorrect);
				NavigationService.Navigate(pageNotification);
			}
    //        else
				//MainWindow.CurrentMainWindow.CloseAllWindows();
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			CheckPatientStateAndShowAppointments(patients[0]);
		}

		private void CheckPatientStateAndShowAppointments(ItemPatient patient) {
			Page pageError = null;

			//if (patient.IsCardBlocked)
			//	pageError = new PageNotification(PageNotification.NotificationType.CardBlocked, returnBack: returnBack);

			//if (patient.IsFirstVisit)
			//	pageError = new PageNotification(PageNotification.NotificationType.FirstVisit, returnBack: returnBack);

			if (pageError != null) {
				NavigationService.Navigate(pageError);
				SetImage(patient, false);
				return;
			}

			DataHandle.UpdatePatientAppointments(ref patient);
            bool isOk = true;

            if (patient.AppointmentsAvailable.Count == 0) {
				PageNotification pageAppointmentsNotFound = 
					new PageNotification(PageNotification.NotificationType.NoAppointmentsForNow, returnBack: returnBack);
				NavigationService.Navigate(pageAppointmentsNotFound);
                isOk = false;
            } else {
				//bool isAnyCash = patient.AppointmentsVisited.Where(e => e.IsCash).Count() > 0;
				//bool isLate = patient.AppointmentsVisited.Where(e => e.IsLate).Count() > 0;
				//bool isAnyRoentgen = patient.AppointmentsVisited.Where(e => e.IsRoentgen).Count() > 0;
				//isOk = !isAnyCash && !isLate && !isAnyRoentgen;

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
