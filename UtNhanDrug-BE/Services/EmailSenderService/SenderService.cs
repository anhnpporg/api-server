using Mailjet.Client;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;
using UtNhanDrug_BE.Configurations;
using UtNhanDrug_BE.Models.EmailModel;

namespace UtNhanDrug_BE.Services.EmailSenderService
{
    public class SenderService : ISenderService
    {

        private readonly EmailConfiguration _emailConfig;
        public SenderService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task SendEmail(MessageModel message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(MessageModel message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Ut Nhan Drug Store", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.Username, _emailConfig.Password);
                await client.SendAsync(mailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //log an error message or throw an exception or both.
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
        //private async Task Page_Load(object sender)
        //{
        //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        //    //create the mail message 
        //    MailMessage mail = new MailMessage();

        //    //set the addresses 
        //    mail.From = new MailAddress("postmaster@utnhandrugstore.ga"); //IMPORTANT: This must be same as your smtp authentication address.
        //    mail.To.Add("postmaster@utnhandrugstore.ga");

        //    //set the content 
        //    mail.Subject = "This is an email";
        //    mail.Body = "This is from system.net.mail using C sharp with smtp authentication.";
        //    //send the message 
        //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mail.utnhandrugstore.ga");

        //    //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
        //    NetworkCredential Credentials = new NetworkCredential("postmaster@utnhandrugstore.ga", "01285980345@a");
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = Credentials;
        //    smtp.Port = 25;    //alternative port number is 8889
        //    smtp.EnableSsl = false;
        //    smtp.SendAsync(mail, null);
        //}


    }
}
