﻿using System;
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
		private readonly string title;

        public PagePatientConfirmation(List<ItemPatient> patients) {
			InitializeComponent();

			this.patients = patients;

			if (patients.Count == 1) {
				TextBlockName.Text = patients[0].Name;
				TextBlockBirthday.Text = "Дата рождения: " + patients[0].Birthday.ToLongDateString();
				title = Properties.Resources.title_name_confirm;
			} else {
				GridSinglePatient.Visibility = Visibility.Hidden;
				GridMultiplePatients.Visibility = Visibility.Visible;
				TextBlockButtonWrong.Text = "Закрыть";
				ButtonWrong.HorizontalAlignment = HorizontalAlignment.Stretch;
				Grid.SetColumn(ButtonWrong, 1);
				Grid.SetRow(ButtonWrong, 2);
                ButtonContinue.Visibility = Visibility.Hidden;
				title = Properties.Resources.title_name_confirm_multiple;
				KeepAlive = true;
                DrawPatients();
				returnBack = true;
			}

			DataContext = BindingValues.Instance;
			IsVisibleChanged += (s, e) => {
				if ((bool)e.NewValue)
					BindingValues.Instance.SetUpMainWindow(title, true, false);
			};
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
			if (patients.Count == 1) {
				PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.NameNotCorrect);
				NavigationService.Navigate(pageNotification);
			} else
				MainWindow.Instance.CloseAllWindows();
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			CheckPatientStateAndShowAppointments(patients[0]);
		}

		private void CheckPatientStateAndShowAppointments(ItemPatient patient) {
			DataHandle.GetPatientAppointments(ref patient);
			PageNotification.NotificationType? notificationType = null;

			if (patient.StopCodesCurrent.Contains(ItemPatient.StopCodes.FirstTime))
				notificationType = PageNotification.NotificationType.FirstVisit;
			else if (patient.StopCodesCurrent.Contains(ItemPatient.StopCodes.NotAvailableNow))
				notificationType = PageNotification.NotificationType.NoAppointmentsForNow;
			else if (patient.StopCodesCurrent.Contains(ItemPatient.StopCodes.Cash))
				notificationType = PageNotification.NotificationType.Cash;
			else if (patient.StopCodesCurrent.Count > 0)
				notificationType = PageNotification.NotificationType.VisitRegistryToCheckIn;

			if (notificationType.HasValue) {
				NavigationService.Navigate(new PageNotification(notificationType.Value, returnBack: returnBack));
				SetImage(patient, false);
				return;
			}

            bool isOk = true;

            if (patient.AppointmentsAvailable.Count == 0) {
				PageNotification pageAppointmentsNotFound = 
					new PageNotification(PageNotification.NotificationType.NoAppointmentsForNow, returnBack: returnBack);
				NavigationService.Navigate(pageAppointmentsNotFound);
                isOk = false;
            } else {
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
