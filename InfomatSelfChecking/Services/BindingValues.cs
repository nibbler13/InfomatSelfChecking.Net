﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace InfomatSelfChecking {
    sealed class BindingValues : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private static BindingValues instance = null;
        private static readonly object padlock = new object();

        public static BindingValues Instance {
            get {
                lock (padlock) {
                    if (instance == null) 
                        instance = new BindingValues();

                    return instance;
                }
            }
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

		public double FontSizeAboutDeveloper {
			get {
				return FontSizeMain * 0.4;
			}
		}

		public double FontSizeSubNotification {
			get {
				return FontSizeMain * 0.8;
			}
		}

		public double FontSizePatientName {
			get {
				return FontSizeMain * 1.2;
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
            private set {
                if (value != brushTitleBackground) {
                    brushTitleBackground = value;
                    NotifyPropertyChanged();
                }
            }
        }



        private string title;
        public string Title {
            get {
                return title;
            }
            private set {
                if (value != title) {
                    title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string clockHours;
        public string ClockHours {
            get {
                return clockHours;
            }
            private set {
                if (value != clockHours) {
                    clockHours = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string clockMinutes;
        public string ClockMinutes {
            get {
                return clockMinutes;
            }
            private set {
                if (value != clockMinutes) {
                    clockMinutes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility clockSplitterVisibility;
        public Visibility ClockSplitterVisibility {
            get {
                return clockSplitterVisibility;
            }
            set {
                if (value != clockSplitterVisibility) {
                    clockSplitterVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility logoVisibility;
        public Visibility LogoVisibility {
            get {
                return logoVisibility;
            }
            private set {
                if (value != logoVisibility) {
                    logoVisibility = value;
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



		private string dialerText;

		private string DialerText {
			get {
				return dialerText;
			}
			set {
				if (value != dialerText) {
					dialerText = value;
					NotifyPropertyChanged();
					for (int i = 0; i <= 9; i++)
						NotifyPropertyChanged("DialerText" + i);
				}
			}
		}
		
		public string DialerText0 {
			get {
				return DialerText.Substring(0, 1);
			}
		}
		public string DialerText1 {
			get {
				return DialerText.Substring(1, 1);
			}
		}
		public string DialerText2 {
			get {
				return DialerText.Substring(2, 1);
			}
		}
		public string DialerText3 {
			get {
				return DialerText.Substring(3, 1);
			}
		}
		public string DialerText4 {
			get {
				return DialerText.Substring(4, 1);
			}
		}
		public string DialerText5 {
			get {
				return DialerText.Substring(5, 1);
			}
		}
		public string DialerText6 {
			get {
				return DialerText.Substring(6, 1);
			}
		}
		public string DialerText7 {
			get {
				return DialerText.Substring(7, 1);
			}
		}
		public string DialerText8 {
			get {
				return DialerText.Substring(8, 1);
			}
		}
		public string DialerText9 {
			get {
				return DialerText.Substring(9, 1);
			}
		}


		private BindingValues() {
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

			InitializeDialerText();
			StartClockTicking();
        }

		public void InitializeDialerText() {
			DialerText = "__________";
		}

		public void UpdateDialerText(string enteredNumber) {
			string result = string.Empty;

			for (int i = 0; i <= 9; i++) {
				string symbol = "_";

				if (i < enteredNumber.Length)
					symbol = enteredNumber.Substring(i, 1);

				result += symbol;
			}

			DialerText = result;
		}

		private void StartClockTicking() {
            DispatcherTimer timerSeconds = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1)
            };

            timerSeconds.Tick += (s, e) => {
                Application.Current.Dispatcher.Invoke((Action)delegate {
                    ClockSplitterVisibility = ClockSplitterVisibility == Visibility.Visible ? 
                        Visibility.Hidden : Visibility.Visible;
                    ClockHours = DateTime.Now.Hour.ToString();
                    ClockMinutes = DateTime.Now.ToString("mm");
                });
            };
            timerSeconds.Start();
        }
        
        public void SetUpMainWindow(string title, bool isLogoVisible, bool isError) {
			Title = title;

			LogoVisibility = isLogoVisible ? 
				Visibility.Visible : 
				Visibility.Hidden;

            BrushTitleBackground = isError ? 
                Properties.Settings.Default.BrushTitleErrorBackground :
                Properties.Settings.Default.BrushTitleBackground;
        }
    }
}
