using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Supermarket_System.Service
{
    public static class  SendEmailToManager
    {
        public static void IsSendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var stmpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("your-email@gmail.com", "your-email-password"),
                    EnableSsl = true
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("your-email@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(toEmail);
                stmpClient.Send(mailMessage);
            }catch(Exception ex)
            {
                throw new Exception($"Lỗi khi gửi email: {ex.Message}");
            }
        }
    }
}