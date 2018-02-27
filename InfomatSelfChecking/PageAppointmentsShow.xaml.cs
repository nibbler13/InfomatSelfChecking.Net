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
    /// Логика взаимодействия для PageAppointmentsShow.xaml
    /// </summary>
    public partial class PageAppointmentsShow : Page {
        public PageAppointmentsShow(ItemPatient patient) {
            InitializeComponent();

			int row = 1;
			foreach (ItemAppointment item in patient.Appointments) {
				AddTextBlock(item.BHour + ":" + item.BMin, row, 0);
				AddTextBlock(item.RNum, row, 1);
				AddTextBlock(item.DName, row, 2);
				row++;
			}
        }

		private void AddTextBlock(string text, int row, int column) {
			TextBlock textBlockTime = new TextBlock();
			textBlockTime.Text = text;
			Grid.SetRow(textBlockTime, row);
			Grid.SetColumn(textBlockTime, column);
			GridAppointments.Children.Add(textBlockTime);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {

		}
	}
}
