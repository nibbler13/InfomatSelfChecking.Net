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
		public PageNotification() {
			InitializeComponent();
		}

		private void MediaElementWelcomeAnimation_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}
	}
}
