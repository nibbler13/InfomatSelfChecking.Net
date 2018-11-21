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
    /// Логика взаимодействия для PageAppointmentsShow.xaml
    /// </summary>
    public partial class PageShowAppointments : Page {
		private bool returnBack;

        public PageShowAppointments(ItemPatient patient, bool returnBack) {
            InitializeComponent();

			this.returnBack = returnBack;
			bool isLate = false;
			bool isAnyCash = false;
			bool isAnyRoentgen = false;
			bool showLogo = true;

			MainWindow.ConfigurePage(this);

			//foreach (Button button in new Button[] { ButtonBeNotedAndPrint, ButtonPrintAndClose, ButtonClose }) 
			//	button.FontSize = MainWindow.FontSizeMain * 0.8;
			
			int row = 1;
			foreach (ItemAppointment item in patient.Appointments) {
				int column = 7;
				AddTextBlock(item.DateTimeBegin.ToShortTimeString(), row, 1, HorizontalAlignment.Center);
				AddTextBlock(item.RNum, row, 2, HorizontalAlignment.Center);
				AddTextBlock(item.DepShortName, row, 3, HorizontalAlignment.Center);
				AddTextBlock(item.DName, row, 4, HorizontalAlignment.Left);

				if (item.IsLate) {
					AddImage(ControlsFactory.ImageType.AppointmentsLate, row, column--);
					isLate = true;
				}

				if (item.IsCash) {
					AddImage(ControlsFactory.ImageType.AppointmentsCash, row, column--);
					isAnyCash = true;
				}

				if (item.IsRoentgen) {
					AddImage(ControlsFactory.ImageType.AppointmentsRoentgen, row, column--);
					isAnyRoentgen = true;
				}

				if (item.IsLate || item.IsCash || item.IsRoentgen) {
					Label label = new Label() {
						Background = MainWindow.BrushHeaderErrorBackground,
						Foreground = MainWindow.BrushTextHeaderForeground,
						Content = " ! ",
						VerticalContentAlignment = VerticalAlignment.Center,
						HorizontalContentAlignment = HorizontalAlignment.Center,
						Margin = new Thickness(2, 2, 0, 0)
					};
					Grid.SetRow(label, row);
					GridAppointments.Children.Add(label);
				}

				if (row > 1) {
					Label label = new Label {
						Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
						Height = 5,
						VerticalAlignment = VerticalAlignment.Top,
						Margin = new Thickness(2, 0, 0, 2)
					};
					Grid.SetRow(label, row);
					Grid.SetColumnSpan(label, 8);
					GridAppointments.Children.Add(label);
				}
				
				row++;

				if ((row == 5 && (isLate || isAnyCash || isAnyRoentgen)) || (row == 9))
					break;
			}

			if (isLate || isAnyCash || isAnyRoentgen) {
				row = 6;
				ButtonClose.Visibility = Visibility.Visible;
				ButtonBeNotedAndPrint.Visibility = Visibility.Hidden;
				ButtonPrintAndClose.Visibility = Visibility.Visible;
				showLogo = false;

				Label label = new Label() {
					Content = Properties.Resources.show_appointments_warning,
					Background = Brushes.Orange,
					Foreground = MainWindow.BrushTextHeaderForeground,
					VerticalAlignment = VerticalAlignment.Stretch,
					VerticalContentAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					HorizontalContentAlignment = HorizontalAlignment.Center,
					FontFamily = new FontFamily("Franklin Gothic Book"),
					FontSize = MainWindow.FontSizeMain * 1.0
				};
				Grid.SetRow(label, row++);
				Grid.SetColumnSpan(label, 8);
				GridAppointments.Children.Add(label);

				StackPanel stackPanel = new StackPanel() {
					Orientation = Orientation.Horizontal,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				};

				Dictionary<ControlsFactory.ImageType, string> warnings = new Dictionary<ControlsFactory.ImageType, string>();
				if (isLate)
					warnings.Add(ControlsFactory.ImageType.AppointmentsLate, Properties.Resources.show_appointments_late);
				if (isAnyCash)
					warnings.Add(ControlsFactory.ImageType.AppointmentsCash, Properties.Resources.show_appointments_cash);
				if (isAnyRoentgen)
					warnings.Add(ControlsFactory.ImageType.AppointmentsRoentgen, Properties.Resources.show_appointments_roentgen);

				foreach (KeyValuePair<ControlsFactory.ImageType, string> warning in warnings) {
					Image image = ControlsFactory.CreateImage(warning.Key, margin: new Thickness(5, 10, 5, 10));
					stackPanel.Children.Add(image);

					TextBlock textBlock = ControlsFactory.CreateTextBlock(warning.Value, margin: new Thickness(10, 0, 10, 0));
					textBlock.FontSize = MainWindow.FontSizeMain * 0.9;
					textBlock.FontFamily = new FontFamily("Franklin Gothic Book");
					stackPanel.Children.Add(textBlock);
				}

				Grid.SetRow(stackPanel, row);
				Grid.SetColumnSpan(stackPanel, 8);
				GridAppointments.Children.Add(stackPanel);
			} else {
				//ButtonBeNotedAndPrint.Foreground = MainWindow.BrushTextHeaderForeground;
				//ButtonBeNotedAndPrint.Background = MainWindow.BrushButtonOkBackground;
			}

			string title = Properties.Resources.title_appointments.Replace("*", patient.Name);
			MainWindow.AppMainWindow.SetUpWindow(showLogo, title, false);
        }

		private void AddImage(ControlsFactory.ImageType imageType, int row, int column) {
			Image image = ControlsFactory.CreateImage(imageType, margin: new Thickness(5, 10, 5, 10));
			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
			Grid.SetRow(image, row);
			Grid.SetColumn(image, column);
			GridAppointments.Children.Add(image);
		}

		private void AddTextBlock(string text, int row, int column, HorizontalAlignment horizontalAlignment) {
			TextBlock textBlock = ControlsFactory.CreateTextBlock(text, margin: new Thickness(10, 0, 10, 0), horizontalAlignment: horizontalAlignment);
			textBlock.FontFamily = new FontFamily("Franklin Gothic Book");
			Grid.SetRow(textBlock, row);
			Grid.SetColumn(textBlock, column);
			GridAppointments.Children.Add(textBlock);
		}

		private void ButtonBeNotedAndPrint_CLick(object sender, RoutedEventArgs e) {
			SetMarks();
			Print();
			Close();
		}

		private void ButtonPrintAndClose_Click(object sender, RoutedEventArgs e) {
			Print();
			Close();
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void Close() {
			if (returnBack) {
				NavigationService.GoBack();
			} else {
				MainWindow.AppMainWindow.CloseAllWindows();
			}
		}

		private void Print() {

		}

		private void SetMarks() {

		}
	}
}
