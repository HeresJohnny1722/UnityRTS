using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Responsible for emailing the receiving email address that the player inputs in the crash reporting screen
/// Using an smtp gmail server for sending
/// Has an option for adding files, which we use for the crash report
/// </summary>
public static class UnityGMail
{
    public static void SendMailFromGoogle(string senderEmail, string recipientEmail, string password, string subject, string body, string attachmentPath = null)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(senderEmail);
        mail.To.Add(recipientEmail);
        mail.Subject = subject;
        mail.Body = body;

        if (attachmentPath != null)
        {
            var attachment = new Attachment(attachmentPath);
            mail.Attachments.Add(attachment);
        }

        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
        smtp.Port = 587;
        smtp.Credentials = new NetworkCredential(senderEmail, password) as ICredentialsByHost;
        smtp.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                return true;
            };

        smtp.Send(mail);
    }
}
