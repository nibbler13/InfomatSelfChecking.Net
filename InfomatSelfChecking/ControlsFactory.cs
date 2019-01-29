using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace InfomatSelfChecking {
	class ControlsFactory {
		public enum ImageType {
			AppointmentsLate,
			AppointmentsCash,
			AppointmentsRoentgen,
			NotificationWelcome,
			NotificationDbError,
			NotificationPrinterError,
			NotificationRegistry,
			NotificationNumberNotFound,
			NotificationOk,
            Department,
            Doctor,
            Room
		}

        private static readonly string rootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string localDoctorPhotosPath = Path.Combine(rootFolder, "DoctorsPhotos");
        private static readonly string localDepartmentPhotosPath = Path.Combine(rootFolder, "DepartmentsPhotos");

        public static TextBlock CreateTextBlock(
            string text, 
			TextAlignment textAlignment = TextAlignment.Left, 
			VerticalAlignment verticalAlignment = VerticalAlignment.Center, 
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
			Thickness? margin = null) {
			return new TextBlock {
				Text = text,
				TextAlignment = textAlignment,
				VerticalAlignment = verticalAlignment,
				HorizontalAlignment = horizontalAlignment,
				Margin = (margin != null) ? margin.Value : new Thickness(0)
			};
		}

		public static Button CreateButton(object content, 
			VerticalAlignment verticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
			Thickness? margin = null,
			object tag = null) {
			return new Button {
				Content = content,
				Margin = (margin != null) ? margin.Value : new Thickness(0),
				BorderThickness = new Thickness(0),
				VerticalContentAlignment = verticalAlignment,
				HorizontalContentAlignment = horizontalAlignment,
				Effect = ControlsFactory.CreateDropShadowEffect(),
				Tag = tag
			};
		}

		public static Image CreateImage(ImageType imageType,
			VerticalAlignment verticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
			Thickness? margin = null,
            string searchName = "") {
			Image image = new Image {
				Source = GetImage(imageType, searchName),
				VerticalAlignment = verticalAlignment,
				HorizontalAlignment = horizontalAlignment,
				Margin = (margin != null) ? margin.Value : new Thickness(0)
			};

			RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
			return image;
		}

		public static DropShadowEffect CreateDropShadowEffect() {
			DropShadowEffect effect = new DropShadowEffect {
				Color = Colors.Black,
				Direction = 315,
				BlurRadius = 10,
				ShadowDepth = 5,
				Opacity = 0.2
			};
			return effect;
		}

		public static BitmapImage GetImage(ImageType imageType, string searchName = "") {
			string fileName;

			switch (imageType) {
				case ImageType.AppointmentsLate:
					fileName = "IconLate.png";
					break;
				case ImageType.AppointmentsCash:
					fileName = "IconCash.png";
					break;
				case ImageType.AppointmentsRoentgen:
					fileName = "IconXray.png";
					break;
				case ImageType.NotificationWelcome:
					fileName = "check-animation-v2.gif";
					break;
				case ImageType.NotificationDbError:
					fileName = "PicError.png";
					break;
				case ImageType.NotificationPrinterError:
					fileName = "PicPrinterError.png";
					break;
				case ImageType.NotificationRegistry:
					fileName = "PicRegistry.png";
					break;
				case ImageType.NotificationNumberNotFound:
					fileName = "PicNotFound.jpg";
					break;
				case ImageType.NotificationOk:
					fileName = "PicOk.png";
					break;
                case ImageType.Room:
                    fileName = "PicRoom.png";
                    break;
                case ImageType.Department:
                    return GetImageForDepartment(searchName);
                case ImageType.Doctor:
                    return GetImageForDoctor(searchName);
				default:
					return new BitmapImage();
			}

			return new BitmapImage(new Uri("pack://application:,,,/InfomatSelfChecking;component/Media/" + fileName));
		}

        public static BitmapImage GetImageForDoctor(string name) {
            Logging.ToLog("DataProvider - Поиск фото для сотрудника: " + name);

            try {
                string[] files = Directory.GetFiles(localDoctorPhotosPath, "*.jpg");

                foreach (string file in files) {
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    if (!fileName.Contains(name))
                        continue;

                    return GetBitmapFromFile(file);
                }

                Logging.ToLog("DataProvider - Не удалось найти изображение для сотрудника: " + name);
            } catch (Exception e) {
                Logging.ToLog(e.Message + Environment.NewLine + e.StackTrace);
            }

            return GetBitmapFromFile("pack://application:,,,/InfomatSelfChecking;component/Media/DoctorWithoutAPhoto.png");
        }

        public static BitmapImage GetImageForDepartment(string depname) {
            try {
                string wantedFile = Path.Combine(localDepartmentPhotosPath, depname + ".png");

                if (File.Exists(wantedFile))
                    return GetBitmapFromFile(wantedFile);
            } catch (Exception e) {
                Logging.ToLog("Не удалось открыть файл с изображением: " + e.Message +
                    Environment.NewLine + e.StackTrace);
            }

            return GetBitmapFromFile("pack://application:,,,/InfomatSelfChecking;component/Media/UnknownDepartment.png");
        }

        private static BitmapImage GetBitmapFromFile(string file) {
            if (file.StartsWith("pack"))
                return new BitmapImage(new Uri(file));

            try {
                using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    BitmapImage bitmapImage = new BitmapImage();
                    stream.Seek(0, SeekOrigin.Begin);
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    return bitmapImage;
                }
            } catch (Exception e) {
                Logging.ToLog("DataProvider - GetBitmapFromFile: " + file + " - " + e.Message + Environment.NewLine + e.StackTrace);
                return null;
            }
        }

        private string ClearDoctorName(string name) {
            if (name.Contains('(')) {
                name = name.Substring(0, name.IndexOf('('));
                name = name.TrimEnd(' ');
            }

            return name;
        }

        public static string ClearTimeString(string timeString) {
            string text = string.Empty;
            string[] timeValues = timeString.Split(new string[] { " - " }, StringSplitOptions.None);

            if (timeValues.Length == 2) {
                string partLeft = timeValues[0].TrimStart('0');
                if (partLeft.StartsWith(":"))
                    partLeft = "0" + partLeft;

                string partRight = timeValues[1].TrimStart('0');
                if (partRight.StartsWith(":"))
                    partRight = "0" + partRight;

                text = partLeft + " - " + partRight;
            } else
                text = timeString;

            return text;
        }

        public static string FirstCharToUpper(string input) {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.ToLower();
            return input.First().ToString().ToUpper() + string.Join("", input.Skip(1));
        }
    }
}
