using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Punc.Utils;

namespace Punc
{
    public class ConfirmationService : IConfirmationService
    {
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public ConfirmationService(IConfiguration config,
            IEmailService emailService)
        {
            _config = config;
            _emailService = emailService;
        }

        public async Task<bool> SendRefereeEmailAsync(Timer timer)
        {
            //null check
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            //check all information exists
            if (timer.RefereeEmail == null)
            {
                throw new ArgumentNullException(nameof(timer.RefereeEmail));
            }

            //get the template
            var subject = _config["Email:Templates:Referee:Subject"];
            var sendTemplate = _config["Email:Templates:Referee:Body"];
            var fromAddress = _config["Email:Sender:Address"];
            var fromName = _config["Email:Sender:Name"];

            //fill out the variables
            var vars = new Dictionary<string, string>
            {
                ["Name"] = timer.CustomerName,
                ["LinkOnTime"] = "https://www.google.com",
                ["LinkLate"] = "https://www.bing.com"
            };

            var email = EmailTemplateEngine.GenerateMessage(new MailAddress(fromAddress, fromName), timer.RefereeEmail, subject, sendTemplate,
                vars);

            return await _emailService.SendEmail(email);
        }

        public async Task<string> CreateConfirmationCodeAsync(Guid timerId)
        {
            var code = Guid.NewGuid().ToString("N");

            //store the confirmation code

            return code;
        }

        public async Task<bool> ConfirmPunctualityAsync(Guid timerId, string confirmationCode, bool ontime)
        {
            throw new NotImplementedException();
        }


    }
}
