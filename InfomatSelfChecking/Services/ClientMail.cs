﻿using System.Net.Mail;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InfomatSelfChecking {
	public static class ClientMail {
		public static async void SendMail (string subject, string body, string receiver, string attachmentPath = "") {
			Logging.ToLog("Mail - Отправка сообщения, тема: " + subject + ", текст: " + body);
			Logging.ToLog("Mail - Получатели: " + receiver);

			if (string.IsNullOrEmpty(receiver) ||
				Debugger.IsAttached)
				return;

			try {
				string appName = Assembly.GetExecutingAssembly().GetName().Name;

				MailAddress from = new MailAddress(
					Properties.Settings.Default.MailUser + "@" + 
					Properties.Settings.Default.MailDomain, appName);

				List<MailAddress> mailAddressesTo = new List<MailAddress>();

				if (receiver.Contains(";")) {
					string[] receivers = receiver.Split(';');
					foreach (string address in receivers)
						mailAddressesTo.Add(new MailAddress(address));
				} else
					mailAddressesTo.Add(new MailAddress(receiver));
				
				body += Environment.NewLine + Environment.NewLine + 
					"___________________________________________" + Environment.NewLine +
					"Это автоматически сгенерированное сообщение" + Environment.NewLine + 
					"Просьба не отвечать на него" + Environment.NewLine +
 					"Имя системы: " + Environment.MachineName;

				MailMessage message = new MailMessage();

				foreach (MailAddress mailAddress in mailAddressesTo)
					message.To.Add(mailAddress);

				message.IsBodyHtml = body.Contains("<") && body.Contains(">");

				if (message.IsBodyHtml)
					body = body.Replace(Environment.NewLine, "<br>");
				
				if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath)) {
#pragma warning disable CA2000 // Dispose objects before losing scope
#pragma warning disable IDE0068 // Use recommended dispose pattern
					Attachment attachment = new Attachment(attachmentPath);
#pragma warning restore IDE0068 // Use recommended dispose pattern
#pragma warning restore CA2000 // Dispose objects before losing scope

					if (message.IsBodyHtml && attachmentPath.EndsWith(".jpg")) {
						attachment.ContentDisposition.Inline = true;

						LinkedResource inline = new LinkedResource(attachmentPath, MediaTypeNames.Image.Jpeg) {
							ContentId = Guid.NewGuid().ToString()
						};

						body = body.Replace("Фотография с камеры терминала:", "Фотография с камеры терминала:<br>" +
							string.Format(@"<img src=""cid:{0}"" />", inline.ContentId));

						AlternateView avHtml = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
						avHtml.LinkedResources.Add(inline);

						message.AlternateViews.Add(avHtml);
					} else
						message.Attachments.Add(attachment);
				}

				message.From = from;
				message.Subject = subject;
				message.Body = body;

				if (!string.IsNullOrEmpty(Properties.Settings.Default.MailCopy))
					message.CC.Add(Properties.Settings.Default.MailCopy);

				SmtpClient client = new SmtpClient(Properties.Settings.Default.MailSmtpServer, 587) {
					UseDefaultCredentials = false,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					EnableSsl = false,
					Credentials = new System.Net.NetworkCredential(
					Properties.Settings.Default.MailUser,
					Properties.Settings.Default.MailPassword)
				};

				await Task.Run(() => { client.Send(message); }).ConfigureAwait(false);
				client.Dispose();

				foreach (Attachment item in message.Attachments)
					item.Dispose();

				message.Dispose();
				Logging.ToLog("Mail - Письмо отправлено успешно");
			} catch (Exception e) {
				Logging.ToLog("SendMail exception: " + e.Message + Environment.NewLine + e.StackTrace);
			}
		}
	}
}
