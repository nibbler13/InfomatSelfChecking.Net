using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
		private static MainWindow instance = null;
		private static readonly object padlock = new object();

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

				Logging.ToLog("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				PrintingSystem.Instance.CloseExcel();
				Application.Current.Shutdown();
			};
            
            StartCheckDbAvailability();
			_ = PrintingSystem.Instance;

			PageNotification pageNotification = new PageNotification(PageNotification.NotificationType.Welcome);
			FrameMain.Navigate(pageNotification);
            DataContext = BindingValues.Instance;
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
			while (FrameMain.NavigationService.CanGoBack) {
				FrameMain.NavigationService.GoBack();
				FrameMain.NavigationService.RemoveBackEntry();
			}
		}
	}
}
