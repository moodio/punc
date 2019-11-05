using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Punc.Utils;

namespace Punc
{
    internal class TimerEmailsService : ITimerEmailsService
    {
        private readonly IConfiguration _config;
        private readonly IRefereeCodesService _refereeCodesService;
        private readonly IEmailService _emailService;

        public TimerEmailsService(IConfiguration config,
            IRefereeCodesService refereeCodesService,
            IEmailService emailService)
        {
            _config = config;
            _refereeCodesService = refereeCodesService;
            _emailService = emailService;
        }

        /// <summary>
        /// Send an email to the referee
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
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

            //create the confirmation code
            var confirmationCode = await _refereeCodesService.CreateRefereeCodeAsync(timer.Id);

            //get the template
            var subject = _config["Email:Templates:Referee:Subject"];
            var sendTemplate = _config["Email:Templates:Referee:Body"];
            var fromAddress = _config["Email:Sender:Address"];
            var fromName = _config["Email:Sender:Name"];

            //links template and variables
            var linkTemplate = _config["Email:Templates:Referee:LinkConfirmationTemplate"];
            var linkVars = new Dictionary<string, string>()
            {
                ["TimerId"] = timer.Id.ToString(),
                ["ConfirmationCode"] = confirmationCode,
                ["OnTimeValue"] = "true"
            };
            var ontimeLink = EmailTemplateEngine.ReplaceTokens(linkTemplate, linkVars);

            //create late link
            linkVars["OnTimeValue"] = "false";
            var lateLink = EmailTemplateEngine.ReplaceTokens(linkTemplate, linkVars);


            //fill out the variables for email
            var vars = new Dictionary<string, string>
            {
                ["LinkOnTime"] = ontimeLink,
                ["LinkLate"] = lateLink,
                ["LinkHomepage"] = _config["Email:Templates:Referee:LinkHomepage"],
                ["CustomerName"] = timer.CustomerName
            };

            var email = EmailTemplateEngine.GenerateMessage(new MailAddress(fromAddress, fromName), timer.RefereeEmail, subject, sendTemplate,
                vars);

            return await _emailService.SendEmail(email);
        }
    }
}
