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
    /// Логика взаимодействия для PagePatientSelectedSingle.xaml
    /// </summary>
    public partial class PagePatientSelectedSingle : Page {
		private ItemPatient patient;

        public PagePatientSelectedSingle(ItemPatient patient) {
            InitializeComponent();
			this.patient = patient;
        }

		private void ButtonWrong_Click(object sender, RoutedEventArgs e) {
			while(NavigationService.CanGoBack) {
				NavigationService.GoBack();
				NavigationService.RemoveBackEntry();
			}
		}

		private void ButtonContinue_Click(object sender, RoutedEventArgs e) {
			patient.Appointments = DataHandle.GetAppointments(patient.PCode);

			if (patient.Appointments.Count == 0) {
				PageAppointmentsNotFound pageAppointmentsNotFound = new PageAppointmentsNotFound();
				NavigationService.Navigate(pageAppointmentsNotFound);
			} else {
				PageAppointmentsShow pageAppointmentsShow = new PageAppointmentsShow(patient);
				NavigationService.Navigate(pageAppointmentsShow);
			}
		}
	}
}
