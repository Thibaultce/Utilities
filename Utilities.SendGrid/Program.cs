using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Utilities.SendGrid
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .AddUserSecrets<Program>()
              .Build();

            try
            {
                var client = new SendGridClient(config["SendGrid:Key"]);
                var msg = MailHelper.CreateSingleEmail(
                    new EmailAddress(config["Email:From"]),
                    new EmailAddress(config["Email:To"]),
                    config["Email:Subject"],
                    config["Email:Body:PlainText"],
                    config["Email:Body:Html"]);
                var response = await client.SendEmailAsync(msg);

                Console.WriteLine("E-mail sent !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
