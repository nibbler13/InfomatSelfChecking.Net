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
	public partial class MainWindow : Window {
		public static MainWindow AppMainWindow { get; private set; }
		public static double FontSizeMain { get; private set; } = 10;
		public static FontFamily FontFamilyMain { get; private set; } = new FontFamily("Franklin Gothic");
		public static FontFamily FontFamilySub { get; private set; } = new FontFamily("Franklin Gothic Book");
		public static Brush BrushTextForeground { get; private set; } = new SolidColorBrush(Color.FromRgb(44, 61, 63));
		public static Brush BrushTextDisabledForeground { get; private set; } = new SolidColorBrush(Color.FromRgb(165, 165, 165));
		public static Brush BrushTextHeaderForeground { get; private set; } = Brushes.White;
		public static Brush BrushHeaderErrorBackground { get; private set; } = new SolidColorBrush(Color.FromRgb(249, 141, 60));
		public static Brush BrushHeaderBackground { get; private set; } = new SolidColorBrush(Color.FromRgb(78, 155, 68));

		public MainWindow() {
			InitializeComponent();

			KeyDown += (s, e) => {
				if (!e.Key.Equals(Key.Escape))
					return;

				Logging.LogMessageToFile("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			};

			AppMainWindow = this;
			
			TextBlockTimeHours.Foreground = BrushTextHeaderForeground;
			TextBlockTimeSplitter.Foreground = BrushTextHeaderForeground;
			TextBlockTimeMinutes.Foreground = BrushTextHeaderForeground;

			FontFamily = FontFamilyMain;
			FontWeight = FontWeights.Light;
			LabelTitle.Foreground = BrushTextHeaderForeground;
			LabelTitle.FontWeight = FontWeights.DemiBold;
			
			FrameMain.Foreground = BrushTextForeground;
		
			Loaded += (o, e) => {
				FontSize = LabelTitle.ActualHeight / 3;
				FontSizeMain = FontSize * 0.8;
				TextBlockTimeHours.FontSize = FontSizeMain;
				TextBlockTimeSplitter.FontSize = FontSizeMain;
				TextBlockTimeMinutes.FontSize = FontSizeMain;
			};

			DispatcherTimer timerSeconds = new DispatcherTimer {
				Interval = TimeSpan.FromSeconds(1)
			};

			timerSeconds.Tick += (s, e) => {
				Application.Current.Dispatcher.Invoke((Action)delegate {
					TextBlockTimeSplitter.Visibility = TextBlockTimeSplitter.Visibility == 
					Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
					TextBlockTimeHours.Text = DateTime.Now.Hour.ToString();
					TextBlockTimeMinutes.Text = DateTime.Now.ToString("mm");
				});
			};
			timerSeconds.Start();

			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.Welcome);
			FrameMain.Navigate(pageNotification);
		}

		public void SetUpWindow(bool isLogoVisible, string title, bool isError) {
			ImageLogo.Visibility = isLogoVisible ? Visibility.Visible : Visibility.Hidden;
			TextBlockTitle.Text = title;
			Brush brush;

			if (isError)
				brush = BrushHeaderErrorBackground;
			else
				brush = BrushHeaderBackground;

			LabelTitle.Background = brush;
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
