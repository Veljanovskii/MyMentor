using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyMentor.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using(var message = new MailMessage())
            {
                message.To.Add(new MailAddress(email, email));
                message.From = new MailAddress("noReply.myMentor@gmail.com", "MyMentor");
                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("noReply.myMentor@gmail.com", "apitaakknnokcgwx");//FurgdkbPdhW9Ba9c");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
