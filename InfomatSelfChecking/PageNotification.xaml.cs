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
    /// Логика взаимодействия для PagePatientNotFound.xaml
    /// </summary>
    public partial class PageNotification : Page {
		public enum NotificationType {
			Welcome,
			NumberNotFound,
			NameNotCorrect,
			FirstVisit,
			CardBlocked,
			NoAppointmentsForNow,
			TooManyPatients
		}

		private NotificationType type;
		private bool isError;
		private bool returnBack;
		private string title;

		public PageNotification(NotificationType type, string replacement = "", bool returnBack = false) {
            InitializeComponent();

			this.type = type;
			this.returnBack = returnBack;
			MainWindow.ConfigurePage(this);
			TextBlockTapToContinue.Foreground = MainWindow.BrushTextDisabledForeground;

			string textTop = string.Empty;
			string textBottom = string.Empty;
			ControlsFactory.ImageType imageType = ControlsFactory.ImageType.NotificationRegistry;
			string textToShow = string.Empty;
			title = Properties.Resources.title_notification;
			isError = true;
			
			switch (type) {
				case NotificationType.Welcome:
					textToShow = Properties.Resources.notification_welcome;
					title = Properties.Resources.title_welcome;
					MediaElementWelcomeAnimation.Visibility = Visibility.Visible;
					TextBlockAboutDeveloper.Visibility = Visibility.Visible;
					ImageCenter.Visibility = Visibility.Hidden;
					KeepAlive = true;
					isError = false;
					break;
				case NotificationType.NumberNotFound:
					textToShow = Properties.Resources.notification_nothing_found;
					imageType = ControlsFactory.ImageType.NotificationNumberNotFound;
					break;
				case NotificationType.NoAppointmentsForNow:
					textToShow = Properties.Resources.notification_no_appointmetns_for_now;
					break;
				case NotificationType.NameNotCorrect:
					textToShow = Properties.Resources.notification_wrong_name;
					break;
				case NotificationType.FirstVisit:
					textToShow = Properties.Resources.notification_first_visit;
					break;
				case NotificationType.CardBlocked:
					textToShow = Properties.Resources.notification_need_go_to_registry;
					break;
				default:
					break;
			}

			ImageCenter.Source = ControlsFactory.GetImage(imageType);
			string[] splittedTextToShow = textToShow.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
			TextBlockTop.Text = splittedTextToShow[0].Replace("*", replacement);
			if (splittedTextToShow.Length > 1)
				TextBlockBottom.Text = splittedTextToShow[1];

			PreviewMouseDown += PageNotification_PreviewMouseDown;
			Loaded += PageNotification_Loaded;
		}

		private void PageNotification_Loaded(object sender, RoutedEventArgs e) {
			if (type == NotificationType.Welcome) {
				MainWindow.ConfigurePage(this);
				TextBlockAboutDeveloper.Foreground = MainWindow.BrushTextDisabledForeground;
				TextBlockAboutDeveloper.FontSize = FontSize / 2;
			}

			MainWindow.AppMainWindow.SetUpWindow(true, title, isError);
		}

		private void PageNotification_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (type == NotificationType.Welcome) {
				PageEnterNumber pageEnterNumber = new PageEnterNumber();
				NavigationService.Navigate(pageEnterNumber);
				return;
			}

			if (returnBack)
				NavigationService.GoBack();
			else
				MainWindow.AppMainWindow.CloseAllWindows();
		}

		private void MediaElementWelcomeAnimation_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}
	}
}
