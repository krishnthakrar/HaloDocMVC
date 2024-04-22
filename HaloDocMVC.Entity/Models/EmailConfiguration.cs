﻿using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace HaloDocMVC.Entity.Models
{
    public class EmailConfiguration
    {
        public string? From { get; set; }
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }

        #region SendMail
        public async Task<bool> SendMail(String To, String Subject, String Body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", From));
                message.To.Add(new MailboxAddress("", To));
                message.Subject = Subject;
                message.Body = new TextPart("html")
                {
                    Text = Body
                };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync(SmtpServer, Port, false);
                    await client.AuthenticateAsync(UserName, Password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region SendMailAsync
        public async Task<bool> SendMailAsync(string To, string Subject, string Body, List<string> Attachments)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("", From));
            message.To.Add(new MailboxAddress("", To));
            message.Subject = Subject;

            // Create the multipart/mixed container to hold the message body and attachments
            var multipart = new Multipart("mixed");

            // Create HTML body part
            var bodyPart = new TextPart("html")
            {
                Text = Body
            };
            multipart.Add(bodyPart);

            if (Attachments != null)
            {
                foreach (string attachmentPath in Attachments)
                {
                    if (!string.IsNullOrEmpty(attachmentPath) && File.Exists(attachmentPath))
                    {
                        // Create MimePart for attachment
                        var attachment = new MimePart()
                        {
                            Content = new MimeContent(File.OpenRead(attachmentPath), ContentEncoding.Default),
                            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                            ContentTransferEncoding = ContentEncoding.Base64,
                            FileName = Path.GetFileName(attachmentPath)
                        };

                        // Add attachment to multipart container
                        multipart.Add(attachment);
                    }
                }
            }

            // Set the message body to the multipart container
            message.Body = multipart;

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(SmtpServer, Port, false);
                await client.AuthenticateAsync(UserName, Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            return true;
        }
        #endregion

        #region Encode_Decode
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
        #endregion

        #region SendSMS
        public async Task<bool> SendSMS(string receiverPhoneNumber, string message)
        {
            string accountSid = "ACb392b446511c6c2da34ba0567a9da2d9";
            string authToken = "fd6cbab005b2aacdc408553f698a0a63";
            string twilioPhoneNumber = "+12513195946";

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var smsMessage = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(receiverPhoneNumber)
                );

                Console.WriteLine("SMS sent successfully. SID: " + smsMessage.Sid);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while sending the SMS: " + ex.Message);
            }
            return false;
        }
        #endregion
    }
}
