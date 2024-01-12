using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MimeKit;
using System.IO;
using MailKit.Net.Smtp;
using MailKit.Security;

public static class Emailer
{
    public static void SendEmail()
    { 

        /*
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("HH - Email Sender", YOUR_SENDER_EMAIL_USERNAME));
        message.To.Add(new MailboxAddress("HH - Email Receiver", YOUR_RECEIVER_EMAIL_USERNAME));
        message.Subject = "HH Help Email Subject";

        var multipartBody = new Multipart("mixed");
        {
            var textPart = new TextPart("plain")
            {
                Text = @"Message Text here!"
            };
            multipartBody.Add(textPart);

            string attachmentPath = IMAGE_PATH;
            var attachmentPart = new MimePart("image/png")
            {
                Content = new MimeContent(File.OpenRead(attachmentPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(attachmentPath)
            };
            multipartBody.Add(attachmentPart);

            string logPath = LOG_PATH;
            var logPart = new MimePart("text/plain")
            {
                Content = new MimeContent(File.OpenRead(logPath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(logPath)
            };
            multipartBody.Add(logPart);
        }
        message.Body = multipartBody;

        using (var client = new SmtpClient())
        {
            // This section must be changed based on your sender's email host
            // Do not use Gmail
            client.Connect("smtp-mail.outlook.com", 587, false);

            client.AuthenticationMechanisms.Remove("XOAUTH2");
            client.Authenticate(YOUR_SENDER_EMAIL_USERNAME, YOUR_SENDER_EMAIL_PASSWORD);
            client.Send(message);
            client.Disconnect(true);
            Debug.Log("Sent email");
        }
        */
    }
}
