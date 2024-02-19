using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public class EmailSender : IEmailSender
    {
        public string Contraseña { get; set; }
        public EmailSender(IConfiguration config)
        {
            Contraseña = config.GetValue<string>("Email:Clave");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string myemail = "giiip00001@gmail.com";
            string PassWord = Contraseña;
            string alias = "Pinesaples";
            string[] rutas;
            MailMessage myMessage;

            myMessage = new MailMessage();
            myMessage.From = new MailAddress(myemail, alias, Encoding.UTF8);
            myMessage.To.Add(email);
            myMessage.Subject = subject;
            myMessage.Body = htmlMessage;
            myMessage.IsBodyHtml = true;
            myMessage.Priority = MailPriority.High;
            // myMessage.Attachments.Add(new Attachment("C:\\Users\\giip0\\Downloads\\adjunto.txt"));

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Port = 25;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Credentials = new System.Net.NetworkCredential(myemail, PassWord);
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s,
                    X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                { return true; };
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(myMessage).ConfigureAwait(false);
            }

        }
    }
}
