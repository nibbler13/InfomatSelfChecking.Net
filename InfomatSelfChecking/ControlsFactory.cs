using System;
using System.Collections.Generic;
using System.Linq;
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
			NotificationOk
		}

		public static TextBlock CreateTextBlock(string text, 
			TextAlignment textAlignment = TextAlignment.Center, 
			VerticalAlignment verticalAlignment = VerticalAlignment.Center, 
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center,
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
			Thickness? margin = null) {
			Image image = new Image {
				Source = GetImage(imageType),
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
				BlurRadius = 5,
				ShadowDepth = 5,
				Opacity = 0.3
			};
			return effect;
		}

		public static BitmapImage GetImage(ImageType imageType) {
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
					fileName = "PicError.jpg";
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
				default:
					return new BitmapImage();
			}

			return new BitmapImage(new Uri("pack://application:,,,/InfomatSelfChecking;component/Media/" + fileName));
		}
	}
}
