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
	public partial class MainView : Window {

		public MainView() {
			InitializeComponent();

			KeyDown += (s, e) => {
				if (!e.Key.Equals(Key.Escape))
					return;

				Logging.ToLog("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			};

			DataContext = MainViewModel.Instance;
			MainViewModel.Instance.SetNavigationService(FrameMain.NavigationService);
		}
	}
}
