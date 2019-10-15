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
    /// Логика взаимодействия для PageAppointmentsShow.xaml
    /// </summary>
    public partial class PageShowAppointments : Page {
		private readonly bool returnBack;
		private bool? checkInStatus;
		private readonly List<string> schedIds;
		private readonly ItemPatient patient;
		private bool isFirstLoad = true;

        public PageShowAppointments(ItemPatient patient, bool returnBack) {
            InitializeComponent();

			Logging.ToLog("PageShowAppointments - отображение страницы со списком назначений");

			this.returnBack = returnBack;
			this.patient = patient ?? throw new ArgumentNullException(nameof(patient));
			schedIds = patient.AppointmentsAvailable.Select(x => x.SchedID).ToList();

			string title = Properties.Resources.title_appointments.Replace("*", patient.Name);
			BindingValues.Instance.SetUpMainWindow(title, true, false);

            int row = 0;
            int totalAppointments = patient.AppointmentsAvailable.Count;
            double fontSize = BindingValues.Instance.FontSizeMain;
            double fontSizeSub = BindingValues.Instance.FontSizeMain * 0.8;
            Brush brushGray = Brushes.LightGray;
            if (totalAppointments == 1) {
                fontSize = BindingValues.Instance.FontSizeMain;
                fontSizeSub = BindingValues.Instance.FontSizeMain;
                brushGray = Brushes.DarkGray;
            }

            foreach (ItemAppointment item in patient.AppointmentsAvailable) {
                string roomString = "Каб. " + item.RNum;
                if (totalAppointments == 1)
                    roomString = "Кабинет " + item.RNum;

                TextBlock textBlockRoom = ControlsFactory.CreateTextBlock(roomString);
                textBlockRoom.FontSize = fontSize;
                textBlockRoom.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockRoom.FontWeight = FontWeights.Light;
                textBlockRoom.Margin = new Thickness(10, 10, 20, 10);
                textBlockRoom.VerticalAlignment = VerticalAlignment.Top;
                Grid.SetRow(textBlockRoom, row);
                Grid.SetColumn(textBlockRoom, 0);
                GridAppointments.Children.Add(textBlockRoom);

                if (totalAppointments == 1) {
                    row++;
                    GridAppointments.RowDefinitions.Add(new RowDefinition());
                    Grid.SetColumnSpan(textBlockRoom, 3);
                    textBlockRoom.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockRoom.Margin = new Thickness(10);
                } else {
					Border borderRoom = new Border {
						HorizontalAlignment = HorizontalAlignment.Right,
						Width = 4,
						Background = new SolidColorBrush(Color.FromRgb(171, 208, 71)),
						Margin = new Thickness(0, 5, 0, 5)
					};
					Grid.SetRow(borderRoom, row);
                    Grid.SetColumn(borderRoom, 0);
                    GridAppointments.Children.Add(borderRoom);
                }

				StackPanel stackPanelTime = new StackPanel {
					Orientation = Orientation.Vertical,
					Margin = new Thickness(20, 10, 20, 10),
					VerticalAlignment = VerticalAlignment.Center
				};
				Grid.SetRow(stackPanelTime, row);
                Grid.SetColumn(stackPanelTime, 1);
                GridAppointments.Children.Add(stackPanelTime);

                TextBlock textBlockTimeStart = ControlsFactory.CreateTextBlock(
                    "с " + ControlsFactory.ClearTimeString(item.DateTimeScheduleBegin),
                    horizontalAlignment: HorizontalAlignment.Right);
                textBlockTimeStart.FontSize = fontSize;
                textBlockTimeStart.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockTimeStart.FontWeight = FontWeights.Light;
                stackPanelTime.Children.Add(textBlockTimeStart);

                TextBlock textBlockTimeEnd = ControlsFactory.CreateTextBlock(
                    "до " + ControlsFactory.ClearTimeString(item.DateTimeScheduleEnd),
                    horizontalAlignment: HorizontalAlignment.Right);
                textBlockTimeEnd.FontSize = fontSizeSub;
                textBlockTimeEnd.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockTimeEnd.FontWeight = FontWeights.Light;
                textBlockTimeEnd.Foreground = brushGray;
                stackPanelTime.Children.Add(textBlockTimeEnd);

                if (totalAppointments == 1) {
                    row++;
                    GridAppointments.RowDefinitions.Add(new RowDefinition());
                    stackPanelTime.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanelTime.Orientation = Orientation.Horizontal;
                    stackPanelTime.Margin = new Thickness(10);
                    Grid.SetColumnSpan(stackPanelTime, 3);
                    Grid.SetColumn(stackPanelTime, 0);
                    textBlockTimeStart.Margin = new Thickness(0, 0, 10, 0);
                    textBlockTimeEnd.Margin = new Thickness(10, 0, 0, 0);
                } else {
					Border borderTime = new Border {
						HorizontalAlignment = HorizontalAlignment.Right,
						Width = 4,
						Background = new SolidColorBrush(Color.FromRgb(171, 208, 71)),
						Margin = new Thickness(0, 5, 0, 5)
					};
					Grid.SetRow(borderTime, row);
                    Grid.SetColumn(borderTime, 1);
                    GridAppointments.Children.Add(borderTime);
                }

				StackPanel stackPanelDoc = new StackPanel {
					Orientation = Orientation.Vertical,
					Margin = new Thickness(20, 10, 20, 10),
					VerticalAlignment = VerticalAlignment.Center
				};
				Grid.SetRow(stackPanelDoc, row);
                Grid.SetColumn(stackPanelDoc, 2);
                GridAppointments.Children.Add(stackPanelDoc);

                TextBlock textBlockDoc = ControlsFactory.CreateTextBlock(item.DName);
                textBlockDoc.FontSize = fontSize;
                textBlockDoc.FontWeight = FontWeights.Light;
                textBlockDoc.FontFamily = BindingValues.Instance.FontFamilyMain;
                stackPanelDoc.Children.Add(textBlockDoc);

                string deptString = item.DepName;
                if (deptString.Length > 45)
                    deptString = deptString.Substring(0, 42) + "...";

                TextBlock textBlockDept = ControlsFactory.CreateTextBlock(deptString);
                textBlockDept.FontSize = fontSizeSub;
                textBlockDept.FontWeight = FontWeights.Light;
                textBlockDept.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockDept.Foreground = brushGray;
                stackPanelDoc.Children.Add(textBlockDept);

                if (totalAppointments == 1) {
                    row++;
                    GridAppointments.RowDefinitions.Add(new RowDefinition());
                    stackPanelDoc.HorizontalAlignment = HorizontalAlignment.Center;
                    stackPanelDoc.Margin = new Thickness(10);
                    Grid.SetColumnSpan(stackPanelDoc, 3);
                    Grid.SetColumn(stackPanelDoc, 0);
                    textBlockDoc.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlockDept.HorizontalAlignment = HorizontalAlignment.Center;
                }

                int timeLeft = item.GetMinutesLeftToBegin();
                string timeLeftString = string.Empty;
                Brush brushTimeLeft = brushGray;

                if (timeLeft >= -5 && timeLeft <= 5) {
                    timeLeftString = "Сейчас";

                    brushTimeLeft = new SolidColorBrush(Color.FromRgb(229, 92, 68));
                } else if (timeLeft > 5) {
                    timeLeftString = "Через " + timeLeft + " " +
						Helper.GetDeclension(timeLeft);

                    if (totalAppointments == 1)
                        timeLeftString = "Начало приёма через " + timeLeft + " " + 
							Helper.GetDeclension(timeLeft);
                }

                if (!string.IsNullOrEmpty(timeLeftString)) {
                    TextBlock textBlockTimeLeft = ControlsFactory.CreateTextBlock(timeLeftString);
                    textBlockTimeLeft.FontSize = fontSizeSub;
                    textBlockTimeLeft.FontWeight = FontWeights.Light;
                    textBlockTimeLeft.FontFamily = BindingValues.Instance.FontFamilyMain;
                    textBlockTimeLeft.Foreground = brushTimeLeft;
                    textBlockTimeLeft.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlockTimeLeft.VerticalAlignment = VerticalAlignment.Bottom;
                    textBlockTimeLeft.Margin = new Thickness(0, 10, 10, 10);
                    Grid.SetRow(textBlockTimeLeft, row);
                    Grid.SetColumn(textBlockTimeLeft, 2);
                    GridAppointments.Children.Add(textBlockTimeLeft);

                    if (totalAppointments == 1) {
                        Grid.SetColumn(textBlockTimeLeft, 0);
                        Grid.SetColumnSpan(textBlockTimeLeft, 3);
                        textBlockTimeLeft.HorizontalAlignment = HorizontalAlignment.Center;
                        textBlockTimeLeft.VerticalAlignment = VerticalAlignment.Center;
                        textBlockTimeLeft.Margin = new Thickness(10);
                    }
                }

                if (row > 0) {
                    GridAppointments.RowDefinitions.Add(new RowDefinition());

                    Label label = ControlsFactory.CreateLabel(row);
                    GridAppointments.Children.Add(label);

                    if (totalAppointments == 1) {
                        Grid.SetRow(label, 0);
                        Label labelBottom = ControlsFactory.CreateLabel(row);
                        labelBottom.VerticalAlignment = VerticalAlignment.Bottom;
                        GridAppointments.Children.Add(labelBottom);
                        Grid.SetRow(stackPanelDoc, 0);
                        Grid.SetRow(textBlockRoom, 1);
                        Grid.SetRow(stackPanelTime, 2);
                        textBlockRoom.Margin = new Thickness(0, 40, 0, 0);
                        stackPanelTime.Margin = new Thickness(0, 0, 0, 40);
                        Label label1 = ControlsFactory.CreateLabel(1);
                        Label label3 = ControlsFactory.CreateLabel(3);
                        GridAppointments.Children.Add(label1);
                        GridAppointments.Children.Add(label3);
                    }
                }

                row++;

                if (row == 6)
                    break;
            }

			DataContext = BindingValues.Instance;
			IsVisibleChanged += PageShowAppointments_IsVisibleChanged;
        }

		private void PageShowAppointments_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue) {
				if (isFirstLoad) {
					isFirstLoad = false;
					return;
				}

				if (returnBack) {
					NavigationService.GoBack();
					NavigationService.RemoveBackEntry();
				}
			}
		}

		private void ButtonCheckIn_CLick(object sender, RoutedEventArgs e) {
			if (!checkInStatus.HasValue)
				try {
					checkInStatus = DataHandle.SetCheckInForAppointments(schedIds);
				} catch (Exception exc) {

					Logging.ToLog(exc.Message + Environment.NewLine + exc.StackTrace);
				}

			if (!checkInStatus.Value) {
				NavigationService.Navigate(
					new PageNotification(PageNotification.NotificationType.CheckInFailed,
						  returnBack: returnBack));
				return;
			}

			NavigationService.Navigate(new PageCheckInCompleted(patient, returnBack));
        }
	}
}
