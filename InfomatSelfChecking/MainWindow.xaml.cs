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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InfomatSelfChecking {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public static MainWindow AppMainWindow { get; private set; }
		public static Brush BrushTextForeground { get; private set; }
		public static double FontSizeMain { get; private set; } = 10;
		public static FontFamily FontFamilyMain { get; private set; }
		public static Brush BrushButtonOkBackground { get; private set; }
		public static Brush BrushTextDisabledForeground { get; private set; }
		public static Brush BrushButtonDisabledBackground { get; private set; }
		public static Brush BrushTextHeaderForeground { get; private set; }
		public static Brush BrushHeaderErrorBackground { get; private set; }


		public MainWindow() {
			InitializeComponent();

			KeyDown += MainWindow_KeyDown;
			AppMainWindow = this;

			FontFamilyMain = new FontFamily(Properties.Settings.Default.FontMain.FontFamily.Name);
			BrushTextHeaderForeground = ConvertColorToBrush(Properties.Settings.Default.ColorTextHeaderForeground);
			TextBlockTimeHours.Foreground = BrushTextHeaderForeground;
			TextBlockTimeSplitter.Foreground = BrushTextHeaderForeground;
			TextBlockTimeMinutes.Foreground = BrushTextHeaderForeground;

			FontFamily = FontFamilyMain;
			FontWeight = FontWeights.Light;
			Background = ConvertColorToBrush(Properties.Settings.Default.ColorMainWindowBackground);
			LabelTitle.Foreground = BrushTextHeaderForeground;
			LabelTitle.FontWeight = FontWeights.DemiBold;

			BrushTextForeground = ConvertColorToBrush(Properties.Settings.Default.ColorTextMainForeground);
			BrushButtonOkBackground = ConvertColorToBrush(Properties.Settings.Default.ColorButtonNextBackground);
			BrushTextDisabledForeground = ConvertColorToBrush(Properties.Settings.Default.ColorTextDisabledForeground);
			BrushButtonDisabledBackground = ConvertColorToBrush(Properties.Settings.Default.ColorButtonDisabled);
			BrushHeaderErrorBackground = ConvertColorToBrush(Properties.Settings.Default.ColorHeaderErrorBackground);
			FrameMain.Foreground = BrushTextForeground;
		
			Loaded += (o, e) => {
				FontSize = LabelTitle.ActualHeight / 3;
				FontSizeMain = FontSize * 0.8;
				TextBlockTimeHours.FontSize = FontSizeMain;
				TextBlockTimeSplitter.FontSize = FontSizeMain;
				TextBlockTimeMinutes.FontSize = FontSizeMain;
			};

			DispatcherTimer timerSeconds = new DispatcherTimer();
			timerSeconds.Interval = TimeSpan.FromSeconds(1);
			timerSeconds.Tick += (s, e) => {
				Application.Current.Dispatcher.Invoke((Action)delegate {
					TextBlockTimeSplitter.Visibility = TextBlockTimeSplitter.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
					TextBlockTimeHours.Text = DateTime.Now.Hour.ToString();
					TextBlockTimeMinutes.Text = DateTime.Now.ToString("mm");
				});
			};
			timerSeconds.Start();

			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.Welcome);
			FrameMain.Navigate(pageNotification);
		}

		private void MainWindow_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape)) {
				Logging.LogMessageToFile("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			}
		}

		public void SetUpWindows(bool isLogoVisible, string title, bool isError) {
			ImageLogo.Visibility = isLogoVisible ? Visibility.Visible : Visibility.Hidden;
			TextBlockTitle.Text = title;
			System.Drawing.Color color;
			if (isError) color = Properties.Settings.Default.ColorHeaderErrorBackground;
			else color = Properties.Settings.Default.ColorHeaderBackground;
			LabelTitle.Background = ConvertColorToBrush(color);
		}

		public static Brush ConvertColorToBrush(System.Drawing.Color color) {
			return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		public void CloseAllWindows() {
			while (FrameMain.NavigationService.CanGoBack) {
				FrameMain.NavigationService.GoBack();
				FrameMain.NavigationService.RemoveBackEntry();
			}
		}

		public static void ConfigurePage(Page page) {
			page.FontFamily = FontFamilyMain;
			page.Foreground = BrushTextForeground;
			try { page.FontSize = FontSizeMain; } catch (Exception) { }
		}
	}
}
