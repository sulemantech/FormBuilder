namespace FormBuilder.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using System.Net;
    using FormBuilder.Extensions;
    using FormBuilder.Helpers;


    public class EmailSender
    {
        private readonly string _templateDirectory;
        private readonly bool _enableSsl;
        public bool _flagEmail;
        public string _flagReason;
                
        public EmailSender(string templateDirectory, bool enableSsl)
        {
            _templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateDirectory);
            _enableSsl = enableSsl;
        }

        public void SendSubmissionNotificationEmail(string toEmail, string subject, string message)
        {
            SendMail(WebConfig.Get("senderemail"), toEmail, subject, message);
        }

        private string GetMailBodyOfTemplate(string templateName)
        {   
            string body = null;
            body = ReadFileFrom(templateName);                       
            return body;
        }


        private string ReadFileFrom(string templateName)
        {
            string htmlFilePath = string.Concat(Path.Combine(_templateDirectory, templateName), ".html");
            string txtFilePath = string.Concat(Path.Combine(_templateDirectory, templateName), ".txt");

            string body = "";

            if (File.Exists(htmlFilePath))
            {
                body = File.ReadAllText(htmlFilePath);
            }
            else if (File.Exists(txtFilePath))
            {
                body = File.ReadAllText(txtFilePath);
            }
            else
            {
                throw new Exception("Email template \"{0}\" is missing".FormatWith(templateName));
            }

            return body;
        }


        private string PrepareMailBodyWith(string templateName, params string[] pairs)
        {
            string body = GetMailBodyOfTemplate(templateName);

            for (var i = 0; i < pairs.Length; i += 2)
            {
                body = body.Replace("<%={0}%>".FormatWith(pairs[i]), pairs[i + 1]);
            }
         
            body = body.Replace("<%=rootUrl%>", UtilityHelper.AbsoluteWebRoot().ToString());

            return body;
        }

        private string PrepareMailMessageWith(string body, params string[] pairs)
        {
            for (var i = 0; i < pairs.Length; i += 2)
            {
                body = body.Replace("<%={0}%>".FormatWith(pairs[i]), pairs[i + 1]);
            }
            
            body = body.Replace("<%=rootUrl%>", UtilityHelper.AbsoluteWebRoot().ToString());

            return body;
        }

        private void SendMail(MailMessage mail)
        {
            
                if (mail.To != null && mail.To.Any())
                {
                    SmtpClient smtp = this.CreateSmtpClient();                    

                    if (UtilityHelper.IsDebugMode())
                    {
                        smtp.Send(mail);
                    }
                    else
                    {

                        try
                        {
                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            //Log Error
                        }
                    }
                }
        }       

        private void SendMail(string fromAddress, string toAddress, string subject, string body)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress, subject, body, WebConfig.Get("sendername")))
            {
                SendMail(mail);
            }
        }

        private void SendMail(string fromAddress, string toAddress, string subject, string body, string fromName)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress, subject, body, fromName))
            {
                SendMail(mail);
            }
        }

        private void SendMail(string fromAddress, string toAddress, string subject, string body, string fileName, MemoryStream attachment)
        {
            using (MailMessage mail = BuildMessageWith(fromAddress, toAddress, subject, body, fileName, attachment))
            {
                SendMail(mail);
            }
        }      

        private SmtpClient CreateSmtpClient()
        {
            SmtpClient smtp = new SmtpClient();
            return smtp;
        }

        private MailMessage BuildMessageWith(string fromAddress, string toAddress, string subject, string body, string FromName)
        {
            var senderAddress = new MailAddress(fromAddress, FromName);
            MailMessage message = new MailMessage
            {
                Sender = senderAddress, // on Behave of When From differs
                From = senderAddress,
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            string[] tos = toAddress.Split(';');                        
            foreach (string to in tos)
            {   
                message.To.Add(new MailAddress(to, FromName));                
            }

            return message;
        }

        private MailMessage BuildMessageWith(string fromAddress, string toAddress, string subject, string body, string fileName, MemoryStream data)
        {
            MailMessage msg = BuildMessageWith(fromAddress, toAddress, subject, body, WebConfig.Get("sendername"));
            var a = new Attachment(data, fileName);
            msg.Attachments.Add(a);
            return msg;
        }

    }    
}