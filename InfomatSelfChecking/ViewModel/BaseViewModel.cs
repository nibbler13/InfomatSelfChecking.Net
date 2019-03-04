using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace InfomatSelfChecking {
	class BaseViewModel : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		private FontFamily fontFamilyMain;
		public FontFamily FontFamilyMain {
			get {
				return fontFamilyMain;
			}
			private set {
				if (value != fontFamilyMain) {
					fontFamilyMain = value;
					NotifyPropertyChanged();
				}
			}
		}

		private double fontSizeMain;
		public double FontSizeMain {
			get {
				return fontSizeMain;
			}
			private set {
				if (value != fontSizeMain) {
					fontSizeMain = value;
					NotifyPropertyChanged();
				}
			}
		}

		private FontWeight fontWeightMain;
		public FontWeight FontWeightMain {
			get {
				return fontWeightMain;
			}
			private set {
				if (value != fontWeightMain) {
					fontWeightMain = value;
					NotifyPropertyChanged();
				}
			}
		}



		private FontFamily fontFamilyTitle;
		public FontFamily FontFamilyTitle {
			get {
				return fontFamilyTitle;
			}
			private set {
				if (value != fontFamilyTitle) {
					fontFamilyTitle = value;
					NotifyPropertyChanged();
				}
			}
		}

		private double fontSizeTitle;
		public double FontSizeTitle {
			get {
				return fontSizeTitle;
			}
			private set {
				if (value != fontSizeTitle) {
					fontSizeTitle = value;
					NotifyPropertyChanged();
				}
			}
		}

		private FontWeight fontWeightTitle;
		public FontWeight FontWeightTitle {
			get {
				return fontWeightTitle;
			}
			private set {
				if (value != fontWeightTitle) {
					fontWeightTitle = value;
					NotifyPropertyChanged();
				}
			}
		}



		public double FontSizeDialer {
			get {
				return FontSizeTitle * 1.5;
			}
		}


		private Brush brushTextForeground;
		public Brush BrushTextForeground {
			get {
				return brushTextForeground;
			}
			private set {
				if (value != brushTextForeground) {
					brushTextForeground = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Brush brushTextDisabledForeground;
		public Brush BrushTextDisabledForeground {
			get {
				return brushTextDisabledForeground;
			}
			private set {
				if (value != brushTextDisabledForeground) {
					brushTextDisabledForeground = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Brush brushTextTitleForeground;
		public Brush BrushTextTitleForeground {
			get {
				return brushTextTitleForeground;
			}
			private set {
				if (value != brushTextTitleForeground) {
					brushTextTitleForeground = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Brush brushTitleErrorBackground;
		public Brush BrushTitleErrorBackground {
			get {
				return brushTitleErrorBackground;
			}
			private set {
				if (value != brushTitleErrorBackground) {
					brushTitleErrorBackground = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Brush brushTitleBackground;
		public Brush BrushTitleBackground {
			get {
				return brushTitleBackground;
			}
			protected set {
				if (value != brushTitleBackground) {
					brushTitleBackground = value;
					NotifyPropertyChanged();
				}
			}
		}




		private Style styleRoundCorner;
		public Style StyleRoundCorner {
			get {
				return styleRoundCorner;
			}
			private set {
				if (value != styleRoundCorner) {
					styleRoundCorner = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Style styleRoundCornerStretch;
		public Style StyleRoundCornerStretch {
			get {
				return styleRoundCornerStretch;
			}
			private set {
				if (value != styleRoundCornerStretch) {
					styleRoundCornerStretch = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Style styleRoundCornerGreen;
		public Style StyleRoundCornerGreen {
			get {
				return styleRoundCornerGreen;
			}
			private set {
				if (value != styleRoundCornerGreen) {
					styleRoundCornerGreen = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Style styleRoundCornerBlue;
		public Style StyleRoundCornerBlue {
			get {
				return styleRoundCornerBlue;
			}
			private set {
				if (value != styleRoundCornerBlue) {
					styleRoundCornerBlue = value;
					NotifyPropertyChanged();
				}
			}
		}

		private Style styleRoundCornerYellow;
		public Style StyleRoundCornerYellow {
			get {
				return styleRoundCornerYellow;
			}
			private set {
				if (value != styleRoundCornerYellow) {
					styleRoundCornerYellow = value;
					NotifyPropertyChanged();
				}
			}
		}


		public BaseViewModel() {
			FontFamilyMain = Properties.Settings.Default.FontFamilyMain;
			FontSizeMain = Properties.Settings.Default.FontSizeMain;
			FontWeightMain = Properties.Settings.Default.FontWeightMain;

			FontFamilyTitle = Properties.Settings.Default.FontFamilyTitle;
			FontSizeTitle = Properties.Settings.Default.FontSizeTitle;
			FontWeightTitle = Properties.Settings.Default.FontWeightTitle;

			BrushTextForeground = Properties.Settings.Default.BrushTextForeground;
			BrushTextDisabledForeground = Properties.Settings.Default.BrushTextDisabledForeground;
			BrushTextTitleForeground = Properties.Settings.Default.BrushTextTitleForeground;
			BrushTitleErrorBackground = Properties.Settings.Default.BrushTitleErrorBackground;
			BrushTitleBackground = Properties.Settings.Default.BrushTitleBackground;

			StyleRoundCorner = Application.Current.FindResource("RoundCorner") as Style;
			StyleRoundCornerStretch = Application.Current.FindResource("RoundCornerStretch") as Style;
			StyleRoundCornerGreen = Application.Current.FindResource("RoundCornerGreen") as Style;
			StyleRoundCornerBlue = Application.Current.FindResource("RoundCornerBlue") as Style;
			StyleRoundCornerYellow = Application.Current.FindResource("RoundCornerYellow") as Style;
		}
	}
}
