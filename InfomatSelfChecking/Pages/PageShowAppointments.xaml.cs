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
using InfomatSelfChecking.Items;

namespace InfomatSelfChecking.Pages {
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
			Console.WriteLine(@"http://CONSTRUCT_PageShowAppointments");

			Logging.ToLog("PageShowAppointments - отображение страницы со списком назначений");

			this.returnBack = returnBack;
			this.patient = patient ?? throw new ArgumentNullException(nameof(patient));
			schedIds = patient.AppointmentsAvailable.Select(x => x.SchedID).ToList();
			
			string title = Properties.Resources.title_appointments.Replace("*", patient.Name); ;

			if (patient.AppointmentsAvailable.Count == 0) {
				ButtonCheckIn.Visibility = Visibility.Hidden;

				if (patient.IsPrinterReady) {
					ButtonPrint.Visibility = Visibility.Visible;
					ButtonClose.Visibility = Visibility.Visible;
				} else {
					ButtonCloseFull.Visibility = Visibility.Visible;
				}
			}

			BindingValues.Instance.SetUpMainWindow(title, true, false);

            int row = 0;
            double fontSize = BindingValues.Instance.FontSizeMain;
            double fontSizeSub = BindingValues.Instance.FontSizeMain * 0.8;
            Brush brushGray = Brushes.LightGray;

			foreach (ItemAppointment item in patient.AppointmentsToShow) {
                TextBlock textBlockRoom = ControlsFactory.CreateTextBlock("Каб. " + item.RNum);
                textBlockRoom.FontSize = fontSize;
                textBlockRoom.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockRoom.FontWeight = FontWeights.Light;
                textBlockRoom.Margin = new Thickness(10, 10, 20, 10);
                textBlockRoom.VerticalAlignment = VerticalAlignment.Top;
                Grid.SetRow(textBlockRoom, row);
                Grid.SetColumn(textBlockRoom, 0);
                GridAppointments.Children.Add(textBlockRoom);

				Border borderRoom = new Border {
					HorizontalAlignment = HorizontalAlignment.Right,
					Width = 4,
					Background = new SolidColorBrush(Color.FromRgb(171, 208, 71)),
					Margin = new Thickness(0, 5, 0, 5)
				};
				Grid.SetRow(borderRoom, row);
				Grid.SetColumn(borderRoom, 0);
				GridAppointments.Children.Add(borderRoom);

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

				Border borderTime = new Border {
					HorizontalAlignment = HorizontalAlignment.Right,
					Width = 4,
					Background = new SolidColorBrush(Color.FromRgb(171, 208, 71)),
					Margin = new Thickness(0, 5, 0, 5)
				};
				Grid.SetRow(borderTime, row);
				Grid.SetColumn(borderTime, 1);
				GridAppointments.Children.Add(borderTime);

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
				textBlockDoc.TextTrimming = TextTrimming.CharacterEllipsis;
                stackPanelDoc.Children.Add(textBlockDoc);

                string deptString = item.DepName;
                if (deptString.Length > 45)
                    deptString = deptString.Substring(0, 42) + "...";

                TextBlock textBlockDept = ControlsFactory.CreateTextBlock(deptString);
                textBlockDept.FontSize = fontSizeSub;
                textBlockDept.FontWeight = FontWeights.Light;
                textBlockDept.FontFamily = BindingValues.Instance.FontFamilyMain;
                textBlockDept.Foreground = brushGray;
				textBlockDept.TextTrimming = TextTrimming.CharacterEllipsis;
                stackPanelDoc.Children.Add(textBlockDept);

                int timeLeft = item.GetMinutesLeftToBegin();
                string timeLeftString = string.Empty;
                Brush brushTimeLeft = brushGray;

                if (timeLeft >= -5 && timeLeft <= 5) {
                    timeLeftString = "Сейчас";

                    brushTimeLeft = new SolidColorBrush(Color.FromRgb(229, 92, 68));
                } else if (timeLeft > 5) {
                    timeLeftString = "Через " + timeLeft + " " +
						NumbersEndingHelper.GetDeclension(timeLeft);
				}

				Border borderDoc = new Border {
					HorizontalAlignment = HorizontalAlignment.Right,
					Width = 4,
					Background = new SolidColorBrush(Color.FromRgb(171, 208, 71)),
					Margin = new Thickness(0, 5, 0, 5)
				};
				Grid.SetRow(borderDoc, row);
				Grid.SetColumn(borderDoc, 2);
				GridAppointments.Children.Add(borderDoc);

				if (!string.IsNullOrEmpty(timeLeftString)) {
                    TextBlock textBlockTimeLeft = ControlsFactory.CreateTextBlock(timeLeftString);
                    textBlockTimeLeft.FontSize = fontSizeSub;
                    textBlockTimeLeft.FontWeight = FontWeights.Light;
                    textBlockTimeLeft.FontFamily = BindingValues.Instance.FontFamilyMain;
                    textBlockTimeLeft.Foreground = brushTimeLeft;
                    textBlockTimeLeft.HorizontalAlignment = HorizontalAlignment.Right;
                    textBlockTimeLeft.VerticalAlignment = VerticalAlignment.Bottom;
                    textBlockTimeLeft.Margin = new Thickness(20,10,10,10);
                    Grid.SetRow(textBlockTimeLeft, row);
                    Grid.SetColumn(textBlockTimeLeft, 3);
                    GridAppointments.Children.Add(textBlockTimeLeft);
                }

				if (item.AlreadyChecked) {
					StackPanel stackPanelChecked = new StackPanel() {
						Orientation = Orientation.Horizontal,
						Margin = new Thickness(20, 17, 10, 10),
						HorizontalAlignment = HorizontalAlignment.Right,
						VerticalAlignment = VerticalAlignment.Top
					};

					Image image = new Image() {
						Source = ControlsFactory.GetImage(ControlsFactory.ImageType.NotificationOk),
						MaxHeight = 32,
						MaxWidth = 32,
						VerticalAlignment = VerticalAlignment.Center
					};
					RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
					stackPanelChecked.Children.Add(image);

					TextBlock textBlockChecked = new TextBlock() {
						Text = "Отмечено ранее",
						FontSize = fontSizeSub,
						FontWeight = FontWeights.Light,
						FontFamily = BindingValues.Instance.FontFamilyMain,
						Foreground = brushGray,
						Margin = new Thickness(10, 0, 0, 0),
						VerticalAlignment = VerticalAlignment.Center
					};
					stackPanelChecked.Children.Add(textBlockChecked);

					Grid.SetRow(stackPanelChecked, row);
					Grid.SetColumn(stackPanelChecked, 3);
					GridAppointments.Children.Add(stackPanelChecked);
				}

                if (row > 0) {
                    GridAppointments.RowDefinitions.Add(new RowDefinition());

                    Border border = ControlsFactory.CreateBorder(row);
                    GridAppointments.Children.Add(border);
                }

                row++;

				if (patient.AppointmentsAvailable.Count == 0 &&
					row == 4)
					break;

				if (row == 6)
                    break;
            }

			DataContext = BindingValues.Instance;
			IsVisibleChanged += (s, e) => {
				if ((bool)e.NewValue) {
					if (isFirstLoad) {
						isFirstLoad = false;
						return;
					}

					if (returnBack) {
						try {
							NavigationService.GoBack();
							NavigationService.RemoveBackEntry();
						} catch (Exception exc) {
							Logging.ToLog(exc.Message + Environment.NewLine + exc.StackTrace);
						}
					}
				}
			};
		}

		~PageShowAppointments() {
			Console.WriteLine(@"http://DECONSTRUCT_PageShowAppointments");
		}

		private void ButtonCheckIn_CLick(object sender, RoutedEventArgs e) {
			if (!checkInStatus.HasValue)
				try {
					checkInStatus = DataHandle.Instance.SetCheckInForAppointments(schedIds);
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

		private void ButtonPrint_Click(object sender, RoutedEventArgs e) {
			NavigationService.Navigate(new PageCheckInCompleted(patient, returnBack, true));
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e) {
			NavigationService.Navigate(
				new PageNotification(PageNotification.NotificationType.AlreadyChecked, returnBack: returnBack));
		}
	}
}
