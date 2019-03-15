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
	/// Interaction logic for PageCheckInCompleted.xaml
	/// </summary>
	public partial class PageCheckInCompleted : Page {
		private ItemPatient patient;
		private bool returnBack;
		public PageCheckInCompleted(ItemPatient patient, bool returnBack) {
			InitializeComponent();
			DataContext = BindingValues.Instance;
			this.patient = patient;
			this.returnBack = returnBack;
			Loaded += PageCheckInCompleted_Loaded;
			string title = Properties.Resources.title_notification;
			BindingValues.Instance.SetUpMainWindow(title, true, false);
		}

		private async void PageCheckInCompleted_Loaded(object sender, RoutedEventArgs e) {
			await Task.Run(() => {
				if (!patient.IsWorksheetCreated)
					patient.BackgroundWorkCompletedEvent.WaitOne();

				PrinterInfo.State printerState = patient.PrintAppointmentsAvailable();

				Application.Current.Dispatcher.Invoke(() => {
					MediaElementsDots.MediaEnded -= MediaElementDots_MediaEnded;
					MediaElementsDots.Stop();
					Grid.SetColumnSpan(MediaElementsDots, 1);
					Grid.SetColumnSpan(TextBlockPrinting, 1);

					TextBlockCheckedIn.Visibility = Visibility.Visible;
					MediaElementCheckedIn.Visibility = Visibility.Visible;
					MediaElementCheckedIn.Position = new TimeSpan(0, 0, 1);
					MediaElementCheckedIn.Play();

					TextBlockThanks.Visibility = Visibility.Visible;

					if (printerState == PrinterInfo.State.Printed ||
						printerState == PrinterInfo.State.NotSelect) {
						if (printerState == PrinterInfo.State.Printed)
							TextBlockCheckedIn.Text += Environment.NewLine + "Список назначений напечатан";

						MediaElementsDots.Visibility = Visibility.Hidden;
						TextBlockPrinting.Visibility = Visibility.Hidden;

						Grid.SetColumn(TextBlockCheckedIn, 0);
						Grid.SetColumn(MediaElementCheckedIn, 0);
						Grid.SetColumnSpan(TextBlockCheckedIn, 2);
						Grid.SetColumnSpan(MediaElementCheckedIn, 2);
					} else {
						TextBlockPrinting.Text = "К сожалению, не удалось распечатать " +
						"список назначений. Информация об ошибке передана " + 
						"в службу технической поддержки";
						MediaElementsDots.Visibility = Visibility.Hidden;
						ImagePrinterError.Visibility = Visibility.Visible;
						TextBlockPrinting.FontSize = BindingValues.Instance.FontSizeSubNotification;

					}

					TextBlockTapToContinue.Visibility = Visibility.Visible;
					IsVisibleChanged += PageCheckInCompleted_IsVisibleChanged;
					MainWindow.Instance.PreviewMouseDown += Instance_PreviewMouseDown;
					MainWindow.Instance.ResetAutoCloseTimer();
				});

			});
		}

		private void PageCheckInCompleted_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (!(bool)e.NewValue)
				MainWindow.Instance.PreviewMouseDown -= Instance_PreviewMouseDown;
		}

		private void Instance_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (returnBack)
				NavigationService.GoBack();
			else
				MainWindow.Instance.CloseAllWindows();
		}

		private void MediaElementDots_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}
	}
}
