using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace InfomatSelfChecking {
	class MainViewModel : BaseViewModel {
		private static MainViewModel instance = null;
		private static readonly object padlock = new object();

		public static MainViewModel Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new MainViewModel();

					return instance;
				}
			}
		}

		private MainViewModel() {
			StartClockTicking();
			StartCheckDbAvailability();
		}

		public void SetNavigationService(NavigationService navigationService) {
			this.navigationService = navigationService;

			PageNotification pageNotification = new PageNotification();// new PageNotification(PageNotification.NotificationType.Welcome);
			navigationService.Navigate(pageNotification);
		}



		private string title;
		public string Title {
			get {
				return title;
			}
			private set {
				if (value != title) {
					title = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string clockHours;
		public string ClockHours {
			get {
				return clockHours;
			}
			private set {
				if (value != clockHours) {
					clockHours = value;
					NotifyPropertyChanged();
				}
			}
		}

		private string clockMinutes;
		public string ClockMinutes {
			get {
				return clockMinutes;
			}
			private set {
				if (value != clockMinutes) {
					clockMinutes = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Visibility clockSplitterVisibility;
		public Visibility ClockSplitterVisibility {
			get {
				return clockSplitterVisibility;
			}
			set {
				if (value != clockSplitterVisibility) {
					clockSplitterVisibility = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Visibility logoVisibility;
		public Visibility LogoVisibility {
			get {
				return logoVisibility;
			}
			private set {
				if (value != logoVisibility) {
					logoVisibility = value;
					NotifyPropertyChanged();
				}
			}
		}

		private NavigationService navigationService;



		private void StartClockTicking() {
			DispatcherTimer timerSeconds = new DispatcherTimer {
				Interval = TimeSpan.FromSeconds(1)
			};

			timerSeconds.Tick += (s, e) => {
				Application.Current.Dispatcher.Invoke((Action)delegate {
					ClockSplitterVisibility = ClockSplitterVisibility == Visibility.Visible ?
						Visibility.Hidden : Visibility.Visible;
					ClockHours = DateTime.Now.Hour.ToString();
					ClockMinutes = DateTime.Now.ToString("mm");
				});
			};
			timerSeconds.Start();
		}

		public void SetUpMainWindow(string title, bool isLogoVisible, bool isError) {
			Title = title;

			LogoVisibility = isLogoVisible ?
				Visibility.Visible :
				Visibility.Hidden;

			BrushTitleBackground = isError ?
				Properties.Settings.Default.BrushTitleErrorBackground :
				Properties.Settings.Default.BrushTitleBackground;
		}


		private void StartCheckDbAvailability() {
			//DispatcherTimer timerDbAvailability = new DispatcherTimer();

			//timerDbAvailability.Tick += (s, e) => {
			//	Logging.ToLog("MainWindow - Проверка доступности БД");

			//	if (navigationService.Content is PageNotification) {
			//		PageNotification currentPage = navigationService.Content as PageNotification;
			//		if (currentPage.CurrentNotificationType == PageNotification.NotificationType.DbError)
			//			return;
			//	}

			//	DispatcherTimer dt = s as DispatcherTimer;
			//	if (dt.Interval == TimeSpan.FromSeconds(0))
			//		dt.Interval = TimeSpan.FromMinutes(1);

			//	try {
			//		DataHandle.CheckDbAvailable();
			//	} catch (Exception exc) {
			//		navigationService.Navigate(
			//			new PageNotification(PageNotification.NotificationType.DbError, exception: exc));
			//	}
			//};

			//timerDbAvailability.Start();
		}

		public void CloseAllWindows() {
			while (navigationService.CanGoBack) {
				navigationService.GoBack();
				navigationService.RemoveBackEntry();
			}
		}
	}
}
