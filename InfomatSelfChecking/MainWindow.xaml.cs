using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
		private static MainWindow instance = null;
		private static readonly object padlock = new object();
		private readonly DispatcherTimer autoCloseTimer;

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

				Logging.ToLog("MainWindow - Закрытие по нажатию клавиши ESC");
				ExcelInterop.Instance.CloseExcel();
				Application.Current.Shutdown();
			};
            
            StartCheckDbAvailability();
			_ = ExcelInterop.Instance;

			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.Welcome);
			FrameMain.Navigate(pageNotification);
            DataContext = BindingValues.Instance;

			autoCloseTimer = new DispatcherTimer();
			autoCloseTimer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.AutoCloseTimerIntervalInSeconds);
			autoCloseTimer.Tick += AutoCloseTimer_Tick;
			FrameMain.Navigated += FrameMain_Navigated;
			PreviewMouseDown += MainWindow_PreviewMouseDown;
		}

		private void MainWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (FrameMain.Content is PageNotification pageNotification)
				if (pageNotification.CurrentNotificationType == PageNotification.NotificationType.Welcome)
					return;

			ResetAutoCloseTimer();
		}

		public void ResetAutoCloseTimer() {
			autoCloseTimer.Stop();
			autoCloseTimer.Start();
		}

		private void FrameMain_Navigated(object sender, NavigationEventArgs e) {
			if (e.Content is PageNotification pageNotification) {
				if (pageNotification.CurrentNotificationType == PageNotification.NotificationType.Welcome ||
					pageNotification.CurrentNotificationType == PageNotification.NotificationType.DbError) {
					autoCloseTimer.Stop();
					return;
				}
			}

			if (e.Content is PageCheckInCompleted) {
				autoCloseTimer.Stop();
				return;
			}

			ResetAutoCloseTimer();
		}

		private void AutoCloseTimer_Tick(object sender, EventArgs e) {
			Logging.ToLog("MainWindow - Автозакрытие страницы по таймеру");
			CloseAllWindows();
		}

		private void StartCheckDbAvailability() {
            DispatcherTimer timerDbAvailability = new DispatcherTimer();
			timerDbAvailability.Tick += TimerDbAvailability_Tick;
			timerDbAvailability.Interval = TimeSpan.FromMinutes(1);
            timerDbAvailability.Start();
			TimerDbAvailability_Tick(timerDbAvailability, new EventArgs());
		}

		private async void TimerDbAvailability_Tick(object sender, EventArgs e) {
			Logging.ToLog("MainWindow - Проверка доступности БД");

			if (FrameMain.NavigationService.Content is PageNotification) {
				PageNotification currentPage = FrameMain.NavigationService.Content as PageNotification;
				if (currentPage.CurrentNotificationType == PageNotification.NotificationType.DbError)
					return;
			}

			try {
				await Task.Run(() => {
					DataHandle.CheckDbAvailable();
				});
			} catch (Exception exc) {
				FrameMain.NavigationService.Navigate(
					new PageNotification(PageNotification.NotificationType.DbError, exception: exc));
			}
		}

		public void CloseAllWindows() {
			Logging.ToLog("MainWindow - закрытие всех страниц");

			while (FrameMain.NavigationService.CanGoBack) {
				FrameMain.NavigationService.GoBack();
				FrameMain.NavigationService.RemoveBackEntry();
			}
		}
	}
}
