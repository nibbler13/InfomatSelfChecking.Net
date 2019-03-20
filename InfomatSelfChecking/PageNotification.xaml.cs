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
    public partial class PageNotification : Page {
		public enum NotificationType {
			Welcome,
			NumberNotFound,
			NameNotCorrect,
			FirstVisit,
			NoAppointmentsForNow,
			Cash,
            DbError,
			VisitRegistryToCheckIn,
			CheckInFailed
		}

		public NotificationType CurrentNotificationType { get; private set; }
		private readonly bool isError;
		private readonly bool returnBack;
		private readonly string title;

		public PageNotification(NotificationType type,
						  string replacement = "",
						  bool returnBack = false,
						  Exception exception = null,
						  PrinterInfo.State? printerState = null) {
            InitializeComponent();

			Logging.ToLog("PageNotification - Отображение страницы типа: " + type);

            CurrentNotificationType = type;
			this.returnBack = returnBack;

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
					imageType = ControlsFactory.ImageType.NotificationAppointmentsNotAvailable;
					break;
				case NotificationType.NameNotCorrect:
					textToShow = Properties.Resources.notification_wrong_name;
					imageType = ControlsFactory.ImageType.NotificationWrongNumber;
					break;
				case NotificationType.FirstVisit:
					textToShow = Properties.Resources.notification_first_visit;
					imageType = ControlsFactory.ImageType.NotificationFirstVisit;
					break;
				case NotificationType.VisitRegistryToCheckIn:
					textToShow = Properties.Resources.notification_need_go_to_registry;
					ButtonInfo.Visibility = Visibility.Visible;
					ButtonInfo.Style = BindingValues.Instance.StyleRoundCorner;
					break;
                case NotificationType.DbError:
                    textToShow = Properties.Resources.notification_db_not_available;
                    title = Properties.Resources.title_welcome;

					TextBlockAboutDeveloper.Visibility = Visibility.Visible;
					imageType = ControlsFactory.ImageType.NotificationDbError;
					TextBlockTapToContinue.Visibility = Visibility.Collapsed;
					TextBlockBottom.Margin = new Thickness(0);
					isError = true;

					SetupErrorNotification(exception);

					break;
				case NotificationType.Cash:
					imageType = ControlsFactory.ImageType.NotificationCash;
					textToShow = Properties.Resources.notification_cash;
					break;
				case NotificationType.CheckInFailed:
					imageType = ControlsFactory.ImageType.NotificationDbError;
					textToShow = Properties.Resources.notification_check_in_failed;
					TextBlockAboutDeveloper.Visibility = Visibility.Visible;
					isError = true;
					break;
				default:
					break;
			}

			ImageCenter.Source = ControlsFactory.GetImage(imageType);

			string[] splittedTextToShow = textToShow.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
			TextBlockTop.Text = splittedTextToShow[0].Replace("*", replacement);

			if (splittedTextToShow.Length > 1)
				TextBlockBottom.Text = splittedTextToShow[1];

            IsVisibleChanged += PageNotification_IsVisibleChanged;

			DataContext = BindingValues.Instance;

			IsVisibleChanged += (s, e) => {
				if ((bool)e.NewValue)
					BindingValues.Instance.SetUpMainWindow(title, true, isError);
			};
		}

		private void SetupInfoNotification() {
			TextBlockTop.Text = Properties.Resources.notification_info;
			ImageCenter.Visibility = Visibility.Hidden;
			Grid.SetRowSpan(TextBlockTop, 4);
			TextBlockTop.HorizontalAlignment = HorizontalAlignment.Left;
			TextBlockTop.VerticalAlignment = VerticalAlignment.Center;
			TextBlockTop.TextAlignment = TextAlignment.Left;
			TextBlockTop.TextWrapping = TextWrapping.Wrap;
			TextBlockTop.FontFamily = BindingValues.Instance.FontFamilyMain;
			TextBlockTop.Margin = new Thickness(100);
			ButtonInfo.Visibility = Visibility.Hidden;
		}

		private void SetupErrorNotification(Exception exception) {
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
				Logging.ToLog("PageNotification - проверка доступности БД");

				try {
					DataHandle.CheckDbAvailable();
					PageNotification_PreviewMouseDown(null, null);
				} catch (Exception exc) {
					Logging.ToLog("PageNotification - " + exc.Message + Environment.NewLine + exc.StackTrace);
				}
			};
			dispatcherTimer.Start();
		}

        private void PageNotification_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (CurrentNotificationType == NotificationType.DbError)
                return;

			if ((bool)e.NewValue)
				MainWindow.Instance.PreviewMouseDown += PageNotification_PreviewMouseDown;
			else
				MainWindow.Instance.PreviewMouseDown -= PageNotification_PreviewMouseDown;
		}

		private void PageNotification_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (NavigationService == null)
                return;

			if (ButtonInfo.Visibility == Visibility.Visible) {
				Point p = e.GetPosition(ButtonInfo);
				if (!(p.X < 0 || p.Y < 0 || 
					p.X > ButtonInfo.ActualWidth || p.Y > ButtonInfo.ActualHeight)) {
					ButtonInfo_Click(ButtonInfo, new RoutedEventArgs());
					return;
				}
			}

			if (CurrentNotificationType == NotificationType.Welcome) {
				PageEnterNumber pageEnterNumber = new PageEnterNumber();
				NavigationService.Navigate(pageEnterNumber);
				return;
			}

			if (returnBack) {
				Logging.ToLog("PageNotification - возврат на предыдущую страницу");
				NavigationService.GoBack();
			} else
				MainWindow.Instance.CloseAllWindows();
		}

		private void MediaElementWelcomeAnimation_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}

		private void ButtonInfo_Click(object sender, RoutedEventArgs e) {
			Logging.ToLog("PageNotification - отображение информации об отказе в отметке");
			SetupInfoNotification();
		}
	}
}
