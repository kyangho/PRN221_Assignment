using System.Net.Mail;
using System.Net;

namespace SignalR.Helper
{
    public class MailHelper
    {
        public async static void SendMail(string email, string body)
        {
            
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "1duckyltt@gmail.com",  // replace with valid value
                    Password = "aaiaunpftlrgosjp"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage();
                message.To.Add(email);
                message.Subject = "(no-reply)";
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("1duckyltt@gmail.com");
                await smtp.SendMailAsync(message);
            }
        }
        public async static void SendMail(string email, string body, Dictionary<Stream, string> files)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "1duckyltt@gmail.com",  // replace with valid value
                    Password = "aaiaunpftlrgosjp"  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage();
                message.To.Add(email);
                message.Subject = "(no-reply)";
                message.Body = body;
                message.IsBodyHtml = true;
                message.From = new MailAddress("1duckyltt@gmail.com");

                
                try
                {
                    foreach (var file in files)
                    {
                        Console.WriteLine(file.Value);
                        message.Attachments.Add(new Attachment(file.Key, file.Value));
                    }
                    await smtp.SendMailAsync(message);
                } catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }
    }
}
