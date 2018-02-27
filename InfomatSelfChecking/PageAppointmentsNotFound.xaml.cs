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

namespace InfomatSelfChecking
{
    /// <summary>
    /// Логика взаимодействия для PageAppointmentsNotFound.xaml
    /// </summary>
    public partial class PageAppointmentsNotFound : Page
    {
        public PageAppointmentsNotFound()
        {
            InitializeComponent();
        }

		private void Button_Click(object sender, RoutedEventArgs e) {
			while (NavigationService.CanGoBack) {
				NavigationService.GoBack();
				NavigationService.RemoveBackEntry();
			}
		}
	}
}
