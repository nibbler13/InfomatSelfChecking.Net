using InfomatSelfChecking.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace InfomatSelfChecking {
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application {
		private string mailTo;

		private void Application_Startup(object sender, StartupEventArgs e) {
			DispatcherUnhandledException += App_DispatcherUnhandledException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			mailTo = InfomatSelfChecking.Properties.Settings.Default.MailTo;
			string msg = "!!!App - Запуск приложения";
			Logging.ToLog(msg);
			Mail.SendMail(msg, msg, mailTo);

			MainWindow window = new MainWindow();
			window.Show();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			HandleException(e.ExceptionObject as Exception);
		}

		private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			HandleException(e.Exception);
		}

		private void HandleException(Exception exception) {
			if (exception != null) {
				string msg = exception.Message + Environment.NewLine + exception.StackTrace;
				Logging.ToLog(msg);
				Mail.SendMail("Необработанное исключение", msg, mailTo);
			}

			Logging.ToLog("!!!App - Аварийное завершение работы");
			ExcelInterop.Instance.CloseExcel();
			Process.GetCurrentProcess().Kill();
		}
	}
}
