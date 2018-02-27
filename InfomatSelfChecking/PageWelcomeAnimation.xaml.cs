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
	/// Логика взаимодействия для PageWelcomeAnimation.xaml
	/// </summary>
	public partial class PageWelcomeAnimation : Page {
		public PageWelcomeAnimation() {
			InitializeComponent();
			PreviewMouseDown += PageWelcomeAnimation_PreviewMouseDown;
		}

		private void PageWelcomeAnimation_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			Console.WriteLine("PageWelcomeAnimation_PreviewMouseDown");
			PageEnterNumber pageEnterNumber = new PageEnterNumber();
			NavigationService.Navigate(pageEnterNumber);
		}

		private void WelcomeAnimation_MediaEnded(object sender, RoutedEventArgs e) {
			(sender as MediaElement).Position = new TimeSpan(0, 0, 1);
			(sender as MediaElement).Play();
		}
	}
}
