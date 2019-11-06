using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Punc
{
    internal class SendGridEmailService : IEmailService
    {
        private readonly string _apiKey;

        public SendGridEmailService(IConfiguration config)
        {
            _apiKey = config["SendGrid:ApiKey"];
        }

        public async Task<bool> SendEmail(MailMessage email)
        {
            var client = new SendGridClient(_apiKey);
            var res = await client.SendEmailAsync(Map(email));

            if ((int)res.StatusCode >= 200 && (int)res.StatusCode < 300)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private SendGridMessage Map(MailMessage email)
        {
            var from = new EmailAddress(email.From.Address, email.From.DisplayName);
            var to = new EmailAddress(email.To.First().Address);
            return MailHelper.CreateSingleEmail(from, to, email.Subject, email.Body, email.Body);
        }
    }
}
