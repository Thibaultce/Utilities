using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.IO;
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
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(config["Smtp:Username"]));
                message.To.Add(new MailboxAddress(config["Email:To"]));
                message.Subject = "How you doin'?";

                message.Body = new TextPart("plain")
                {
                    Text = @"Hey Chandler,
                        I just wanted to let you know that Monica and I were going to go play some paintball, you in?
                        -- Joey"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(config["Smtp:Server"], int.Parse(config["Smtp:Port"]), bool.Parse(config["Smtp:Ssl"]));

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(config["Smtp:Username"], config["Smtp:Password"]);

                    client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                    {
                        return true;
                    };

                    client.CheckCertificateRevocation = false;

                    client.Send(message);
                    client.Disconnect(true);
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
