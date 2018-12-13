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
using System.Windows.Threading;

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
			TooManyPatients,
            DbError
		}

		public NotificationType CurrentNotificationType { get; private set; }
		private readonly bool isError;
		private readonly bool returnBack;
		private readonly string title;

		public PageNotification(NotificationType type, string replacement = "", bool returnBack = false, Exception exception = null) {
            InitializeComponent();

            CurrentNotificationType = type;
			this.returnBack = returnBack;
			MainWindow.ConfigurePage(this);
            TextBlockBottom.Foreground = MainWindow.BrushTextDisabledForeground;
            TextBlockTapToContinue.Foreground = MainWindow.BrushTextDisabledForeground;

			string textTop = string.Empty;
			string textBottom = string.Empty;
			ControlsFactory.ImageType imageType = ControlsFactory.ImageType.NotificationRegistry;
			string textToShow = string.Empty;
			title = Properties.Resources.title_notification;
			
			switch (type) {
				case NotificationType.Welcome:
					textToShow = Properties.Resources.notification_welcome;
					title = Properties.Resources.title_welcome;
					MediaElementWelcomeAnimation.Visibility = Visibility.Visible;
					TextBlockAboutDeveloper.Visibility = Visibility.Visible;
					GridImage.Visibility = Visibility.Hidden;
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
                case NotificationType.DbError:
                    textToShow = Properties.Resources.notification_db_not_available;
                    title = Properties.Resources.title_welcome;
                    TextBlockAboutDeveloper.Visibility = Visibility.Visible;
                    imageType = ControlsFactory.ImageType.NotificationDbError;
                    TextBlockTapToContinue.Visibility = Visibility.Collapsed;
                    TextBlockBottom.Margin = new Thickness(0);
                    isError = true;

                    if (exception != null) {
                        string msg = exception.Message + Environment.NewLine + exception.StackTrace;
                        if (exception.InnerException != null)
                            msg += Environment.NewLine + Environment.NewLine + exception.InnerException.Message + 
                                Environment.NewLine + exception.InnerException.StackTrace;

                        Mail.SendMail("Ошибка в работе инфомата", msg, Properties.Settings.Default.MailTo);
                    }

                    DispatcherTimer dispatcherTimer = new DispatcherTimer() {
                        Interval = TimeSpan.FromMinutes(1)
                    };

                    dispatcherTimer.Tick += (s, e) => {
                        try {
                            DataHandle.CheckDbAvailable();
                            PageNotification_PreviewMouseDown(null, null);
                        } catch (Exception exc) {
                            Logging.ToLog("PageNotification - " + exc.Message + Environment.NewLine + exc.StackTrace);
                        }
                    };
                    dispatcherTimer.Start();

                    break;
				default:
					break;
			}

			ImageCenter.Source = ControlsFactory.GetImage(imageType);
			string[] splittedTextToShow = textToShow.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
			TextBlockTop.Text = splittedTextToShow[0].Replace("*", replacement);

			if (splittedTextToShow.Length > 1)
				TextBlockBottom.Text = splittedTextToShow[1];

			Loaded += PageNotification_Loaded;
            IsVisibleChanged += PageNotification_IsVisibleChanged;
		}

        private void PageNotification_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (CurrentNotificationType == NotificationType.DbError)
                return;

            if ((bool)e.NewValue)
                MainWindow.CurrentMainWindow.PreviewMouseDown += PageNotification_PreviewMouseDown;
            else
                MainWindow.CurrentMainWindow.PreviewMouseDown -= PageNotification_PreviewMouseDown;
        }

        private void PageNotification_Loaded(object sender, RoutedEventArgs e) {
			if (CurrentNotificationType == NotificationType.Welcome || 
                CurrentNotificationType == NotificationType.DbError) {
				MainWindow.ConfigurePage(this);
				TextBlockAboutDeveloper.Foreground = MainWindow.BrushTextDisabledForeground;
				TextBlockAboutDeveloper.FontSize = FontSize * 0.4;
			}

            TextBlockBottom.FontSize = FontSize * 0.8;
            TextBlockTapToContinue.FontSize = FontSize * 0.8;

			MainWindow.CurrentMainWindow.SetUpMainWindow(true, title, isError);
		}

		private void PageNotification_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (NavigationService == null)
                return;

			if (CurrentNotificationType == NotificationType.Welcome) {
				PageEnterNumber pageEnterNumber = new PageEnterNumber();
				NavigationService.Navigate(pageEnterNumber);
				return;
			}

			if (returnBack)
                NavigationService.GoBack();
			else
				MainWindow.CurrentMainWindow.CloseAllWindows();
		}

		private void MediaElementWelcomeAnimation_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}
	}
}
