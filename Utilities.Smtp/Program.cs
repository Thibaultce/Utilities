using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Smtp
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .AddUserSecrets<Program>()
              .Build();

            try
            {
                using (var smtpClient = new SmtpClient(config["Smtp:Server"])
                {
                    Credentials = !string.IsNullOrWhiteSpace(config["Smtp:Username"]) ? new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]) : null,
                    Port = int.Parse(config["Smtp:Port"]),
                    EnableSsl = bool.Parse(config["Smtp:Ssl"])
                })
                using (var msg = new MailMessage
                {
                    Body = File.ReadAllText(@"c:\temp\email.html", Encoding.UTF8),
                    From = new MailAddress(config["Email:From"]),
                    Subject = config["Email:Subject"],
                    IsBodyHtml = true,
                    DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure,
                    
                })
                {
                    msg.To.Add(config["Email:To"]);

                    await smtpClient.SendMailAsync(msg).ConfigureAwait(false);
                }

                Console.WriteLine("E-mail sent !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
