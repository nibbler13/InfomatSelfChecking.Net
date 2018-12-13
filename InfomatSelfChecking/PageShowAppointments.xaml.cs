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
        private readonly ItemPatient patient;

        public PageShowAppointments(ItemPatient patient, bool returnBack) {
            InitializeComponent();

            this.patient = patient;
			this.returnBack = returnBack;
			bool showLogo = true;

			MainWindow.ConfigurePage(this);
            ButtonCheckIn.Style = Application.Current.MainWindow.FindResource("RoundCornerGreen") as Style;

            //Loaded += PageShowAppointments_Loaded;


            int row = 0;
            foreach (ItemAppointment item in patient.AppointmentsAvailable) {
                TextBlock textBlockTime = ControlsFactory.CreateTextBlock(ControlsFactory.ClearTimeString(item.DateTimeSchedule));
                textBlockTime.FontSize = MainWindow.FontSizeMain;// * 0.8;
                Grid.SetRow(textBlockTime, row);
                GridAppointments.Children.Add(textBlockTime);

                TextBlock textBlockRoom = ControlsFactory.CreateTextBlock("Кабинет " + item.RNum);
                textBlockRoom.FontSize = MainWindow.FontSizeMain;// * 0.8;
                textBlockRoom.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetRow(textBlockRoom, row);
                Grid.SetColumn(textBlockRoom, 2);
                GridAppointments.Children.Add(textBlockRoom);

                Image imageDept = ControlsFactory.CreateImage(ControlsFactory.ImageType.Department, margin: new Thickness(0, 20, 0, 20), searchName: item.DepName);
                imageDept.MaxHeight = 80;


                //TextBlock textBlockDept = ControlsFactory.CreateTextBlock(item.DepShortName);
                //textBlockDept.FontSize = MainWindow.FontSizeMain * 0.8;
                //textBlockDept.FontFamily = MainWindow.FontFamilySub;
                //textBlockDept.FontWeight = FontWeights.Light;
                //textBlockDept.TextWrapping = TextWrapping.WrapWithOverflow;
                //textBlockDept.HorizontalAlignment = HorizontalAlignment.Left;
                //textBlockDept.TextAlignment = TextAlignment.Left;
                Grid.SetRow(imageDept, row);
                Grid.SetColumn(imageDept, 4);
                GridAppointments.Children.Add(imageDept);

                Regex regex = new Regex(Regex.Escape(" "));
                string docName = regex.Replace(item.DName, Environment.NewLine, 1);
                TextBlock textBlockDoc = ControlsFactory.CreateTextBlock(docName);
                textBlockDoc.FontSize = MainWindow.FontSizeMain;// * 0.8;
                textBlockDoc.FontWeight = FontWeights.Light;
                textBlockDoc.FontFamily = MainWindow.FontFamilySub;
                textBlockDoc.TextAlignment = TextAlignment.Center;
                textBlockDoc.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetRow(textBlockDoc, row);
                Grid.SetColumn(textBlockDoc, 6);
                GridAppointments.Children.Add(textBlockDoc);

                if (row > 0) {
                    GridAppointments.RowDefinitions.Add(new RowDefinition());

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

                if (row == 6)
                    break;
            }

            string title = Properties.Resources.title_appointments.Replace("*", patient.Name);
			MainWindow.CurrentMainWindow.SetUpMainWindow(showLogo, title, false);
        }

        private void PageShowAppointments_Loaded(object sender, RoutedEventArgs e) {
            double thickness = 8;
            double availableWidth = GridAppointments.ActualWidth;
            double availableHeight = GridAppointments.ActualHeight;
            double elementWidth = (availableWidth - thickness * 6) / 3;
            double elementHeight = (availableHeight - thickness * 4) / 2;

            foreach (ItemAppointment item in patient.AppointmentsAvailable) {
                Border border = new Border {
                    Background = new SolidColorBrush(Color.FromRgb(245, 245, 245)),
                    Width = elementWidth,
                    Height = elementHeight,
                    CornerRadius = new CornerRadius(8),
                    Margin = new Thickness(8)
                };

                Grid grid = new Grid {
                    Margin = new Thickness(10)
                };

                RowDefinition rowDefinition0 = new RowDefinition {
                    Height = new GridLength(0, GridUnitType.Auto)
                };

                RowDefinition rowDefinition1 = new RowDefinition {
                    Height = new GridLength(1, GridUnitType.Star)
                };

                RowDefinition rowDefinition2 = new RowDefinition {
                    Height = new GridLength(2, GridUnitType.Star)
                };

                grid.RowDefinitions.Add(rowDefinition0);
                grid.RowDefinitions.Add(rowDefinition1);
                grid.RowDefinitions.Add(rowDefinition2);

                TextBlock textBlockTimeRoom = ControlsFactory.CreateTextBlock(
                    ControlsFactory.ClearTimeString(item.DateTimeSchedule) + Environment.NewLine + "Кабинет " + item.RNum, margin: new Thickness(0));
                textBlockTimeRoom.FontSize = MainWindow.FontSizeMain * 0.8;
                Grid.SetColumnSpan(textBlockTimeRoom, 2);
                grid.Children.Add(textBlockTimeRoom);

                WrapPanel stackPanelDept = new WrapPanel {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    MaxWidth = elementWidth - 20
                };

                Image imageDept = ControlsFactory.CreateImage(
                    ControlsFactory.ImageType.Department, margin: new Thickness(10), searchName:item.DepName);
                stackPanelDept.Children.Add(imageDept);

                TextBlock textBlockDept = ControlsFactory.CreateTextBlock(item.DepName, margin: new Thickness(10, 0, 0, 0));
                textBlockDept.FontSize = MainWindow.FontSizeMain * 0.6;
                textBlockDept.FontFamily = MainWindow.FontFamilySub;
                textBlockDept.FontWeight = FontWeights.Light;
                textBlockDept.TextWrapping = TextWrapping.WrapWithOverflow;
                stackPanelDept.Children.Add(textBlockDept);

                Grid.SetRow(stackPanelDept, 1);
                grid.Children.Add(stackPanelDept);

                StackPanel stackPanelDoc = new StackPanel {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    MaxWidth = elementWidth - 20
                };

                Image imageDoc = ControlsFactory.CreateImage(
                    ControlsFactory.ImageType.Doctor, margin: new Thickness(10), searchName: item.DName);
                stackPanelDoc.Children.Add(imageDoc);

                string docName = string.Join(Environment.NewLine, item.DName.Split(' '));
                TextBlock textBlockDoc = ControlsFactory.CreateTextBlock(docName, margin: new Thickness(10, 0, 0, 0));
                textBlockDoc.FontSize = MainWindow.FontSizeMain * 0.6;
                textBlockDoc.FontWeight = FontWeights.Light;
                textBlockDoc.FontFamily = MainWindow.FontFamilySub;
                stackPanelDoc.Children.Add(textBlockDoc);

                Grid.SetRow(stackPanelDoc, 2);
                grid.Children.Add(stackPanelDoc);

                border.Child = grid;

                WrapPanelAppointments.Children.Add(border);
            }
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

		private void ButtonCheckIn_CLick(object sender, RoutedEventArgs e) {
            if (returnBack) {
                NavigationService.GoBack();
            } else {
                MainWindow.CurrentMainWindow.CloseAllWindows();
            }
        }
	}
}
