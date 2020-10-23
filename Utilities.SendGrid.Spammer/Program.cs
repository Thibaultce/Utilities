using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.SendGrid.Spammer
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
                var content = File.ReadAllText(config["Email:Body:Path"], Encoding.UTF8);

                List<string> tos = new List<string>();
                if (!string.IsNullOrWhiteSpace(config["Email:Destination:csv"]) && File.Exists(config["Email:Destination:csv"]))
                {
                    using (var reader = new StreamReader(config["Email:Destination:csv"]))
                    {
                        while (!reader.EndOfStream)
                        {
                            var value = reader.ReadLine();
                            if (IsValidEmail(value))
                                tos.Add(value);
                        }
                    }
                }
                else
                {
                    tos.Add(config["Email:Destination:To"]);
                }
                var client = new SendGridClient(config["SendGrid:Key"]);

                foreach (var to in tos)
                {
                    try
                    {
                        var msg = MailHelper.CreateSingleEmail(
                           new EmailAddress(config["Email:From"]),
                           new EmailAddress(to),
                           config["Email:Subject"],
                           content,
                           content);

                        var response = await client.SendEmailAsync(msg);

                        Console.WriteLine($"E-mail sent to {to}!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }


                Console.WriteLine("The job is done !");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
