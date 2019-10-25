using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using InfomatSelfChecking.Pages;
using InfomatSelfChecking.Items;

namespace InfomatSelfChecking {
	public partial class MainWindow : Window {
		private static MainWindow instance = null;
		private static readonly object padlock = new object();
		private readonly DispatcherTimer autoCloseTimer;
		private DateTime dateTimeStart;

		public static MainWindow Instance {
			get {
				lock (padlock) {
					if (instance == null)
						instance = new MainWindow();

					return instance;
				}
			}
		}

		public MainWindow() {
			InitializeComponent();

			instance = this;

			KeyDown += (s, e) => {
				if (!e.Key.Equals(Key.Escape))
					return;

				ShutdownApp("Закрытие по нажатию клавиши ESC");
			};
            
            StartCheckDbAvailability();
			_ = ExcelInterop.Instance;

			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.Welcome);
			FrameMain.Navigate(pageNotification);
            DataContext = BindingValues.Instance;

			autoCloseTimer = new DispatcherTimer {
				Interval = TimeSpan.FromSeconds(Properties.Settings.Default.AutoCloseTimerIntervalInSeconds)
			};

			autoCloseTimer.Tick += AutoCloseTimer_Tick;
			FrameMain.Navigated += FrameMain_Navigated;
			PreviewMouseDown += MainWindow_PreviewMouseDown;

			if (Debugger.IsAttached) {
				Cursor = Cursors.Arrow;
				WindowState = WindowState.Normal;
				Width = 1280;
				Height = 1024;
				Topmost = false;
			}
		}

		private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (FrameMain.Content is Pages.PageNotification pageNotification)
				if (pageNotification.CurrentNotificationType == PageNotification.NotificationType.Welcome)
					return;

			ResetAutoCloseTimer();
		}

		public void ResetAutoCloseTimer() {
			autoCloseTimer.Stop();
			autoCloseTimer.Start();
			Console.WriteLine(@"http://AUTOCLOSETIMER_STARTED");
		}

		private void FrameMain_Navigated(object sender, NavigationEventArgs e) {
			if (e.Content is PageNotification pageNotification) {
				if (pageNotification.CurrentNotificationType == PageNotification.NotificationType.Welcome ||
					pageNotification.CurrentNotificationType == PageNotification.NotificationType.DbError) {
					autoCloseTimer.Stop();
					return;
				}
			}

			//if (e.Content is PageCheckInCompleted) {
			//	autoCloseTimer.Stop();
			//	return;
			//}

			ResetAutoCloseTimer();
		}

		private void AutoCloseTimer_Tick(object sender, EventArgs e) {
			Logging.ToLog("MainWindow - Автозакрытие страницы по таймеру");
			CloseAllWindows();
		}

		private void StartCheckDbAvailability() {
			dateTimeStart = DateTime.Now;
			DispatcherTimer timerDbAvailability = new DispatcherTimer();
			timerDbAvailability.Tick += TimerDbAvailability_Tick;
			timerDbAvailability.Interval = TimeSpan.FromMinutes(1);
            timerDbAvailability.Start();
			TimerDbAvailability_Tick(timerDbAvailability, new EventArgs());
		}

		private async void TimerDbAvailability_Tick(object sender, EventArgs e) {
			if (dateTimeStart.Date != DateTime.Now.Date)
				ShutdownApp("Автоматическое завершение работы");

			if (FrameMain.NavigationService.Content is PageNotification) {
				PageNotification currentPage = FrameMain.NavigationService.Content as PageNotification;
				if (currentPage.CurrentNotificationType == PageNotification.NotificationType.DbError)
					return;
			}

			Logging.ToLog("MainWindow - Проверка доступности БД");

			try {
				await Task.Run(() => { DataHandle.Instance.CheckDbAvailable(); }).ConfigureAwait(false);
			} catch (Exception exc) {
				ShowErrorScreen(exc, false);
			}
		}

		public void ShowErrorScreen(Exception e, bool isQueryException) {
			if (FrameMain.NavigationService.Content is PageNotification) {
				PageNotification currentPage = FrameMain.NavigationService.Content as PageNotification;
				if (currentPage.CurrentNotificationType == PageNotification.NotificationType.DbError)
					return;
			}

			FrameMain.NavigationService.Navigate(
				new PageNotification(PageNotification.NotificationType.DbError, exception: e, isQueryException: isQueryException));
		}

		public void CloseAllWindows() {
			Logging.ToLog("MainWindow - закрытие всех страниц");

			foreach (ItemPatient patient in DataHandle.PatientsCurrent)
				patient.CloseExcelWorkbook();

			try {
				while (FrameMain.NavigationService.CanGoBack) {
					FrameMain.NavigationService.GoBack();
					FrameMain.NavigationService.RemoveBackEntry();
				}
			} catch (Exception e) {
				Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		private void ShutdownApp(string reason) {
			Logging.ToLog("MainWindow - " + reason);
			CloseAllWindows();
			DataHandle.Instance.CloseDbConnections();
			ExcelInterop.Instance.CloseExcel();
			Application.Current.Shutdown();
		}
	}
}
