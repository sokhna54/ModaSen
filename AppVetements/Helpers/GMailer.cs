using System.Net;
using System.Net.Mail;

namespace AppVetements.Helpers
{
    public class GMailer
    {
        public static string GmailUsername { get; set; }
        public static string GmailPassword { get; set; }
        public static string GmailHost { get; set; } = "smtp.gmail.com";
        public static int GmailPort { get; set; } = 587;
        public static bool GmailSSL { get; set; } = true;

        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }

        public void Send()
        {
            SmtpClient smtp = new SmtpClient();
            smtp.Host = GmailHost;
            smtp.Port = GmailPort;
            smtp.EnableSsl = GmailSSL;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(GmailUsername, GmailPassword);

            using (var message = new MailMessage(GmailUsername, ToEmail))
            {
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsHtml;
                smtp.Send(message);
            }
        }

        public static void SendMail(string destinataire, string sujet, string body)
        {
            GMailer mailer = new GMailer();
            mailer.ToEmail = destinataire;
            mailer.Subject = sujet;
            mailer.Body = body;
            mailer.IsHtml = true;
            mailer.Send();
        }
    }
}