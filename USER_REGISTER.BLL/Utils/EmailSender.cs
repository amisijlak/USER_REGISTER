using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Helpers;

namespace USER_REGISTER.BLL.Utils
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost;
        private readonly string _senderEmailAddress;
        private readonly string _senderPassword;
        private readonly bool _enabled;
        private readonly int _smtpPort;
        private readonly bool _useSsl;

        public EmailSender(bool enabled, string smtpHost, int smtpPort, bool useSsl, string senderEmailAddress, string senderPassword)
        {
            this._enabled = enabled;
            this._smtpHost = smtpHost;
            this._smtpPort = smtpPort;
            this._useSsl = useSsl;
            this._senderEmailAddress = senderEmailAddress;
            this._senderPassword = senderPassword;
        }

        public (bool, string) SendEmail(string[] addresses, string subject, string body)
        {
            string errorMessage = null;
            bool success = false;

            if (!_enabled) success = true;
            else
            {
                try
                {
                    using (SmtpClient smtpServer = new SmtpClient(_smtpHost))
                    {
                        using (MailMessage mail = new MailMessage())
                        {
                            mail.From = new MailAddress(_senderEmailAddress);

                            foreach (var address in addresses)
                            {
                                if (!string.IsNullOrEmpty(address))
                                {
                                    mail.To.Add(address.Trim());
                                }
                            }

                            if (!mail.To.Any()) throw new Exception("No Recipients Found!");

                            mail.Subject = subject;
                            mail.Body = body;
                            mail.IsBodyHtml = true;

                            smtpServer.Port = _smtpPort;
                            smtpServer.UseDefaultCredentials = false;
                            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtpServer.Credentials = new System.Net.NetworkCredential(_senderEmailAddress, _senderPassword);
                            smtpServer.EnableSsl = _useSsl;
                            smtpServer.Timeout = 60000;//1 minute
                            smtpServer.Send(mail);
                        }
                    }

                    success = true;
                }
                catch (Exception e)
                {
                    errorMessage = e.ExtractInnerExceptionMessage();
                }
            }

            return (success, errorMessage);
        }
    }
}
