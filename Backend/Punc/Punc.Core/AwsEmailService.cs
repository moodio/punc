using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;

namespace Punc
{
    public class AwsEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public AwsEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmail(MailMessage email)
        {
            var accessKey = _config["AWS:SES:AccessKeyId"];
            var secret = _config["AWS:SES:AccessKeySecret"];
            var creds = new BasicAWSCredentials(accessKey, secret);

            using (var client = new AmazonSimpleEmailServiceClient(creds,RegionEndpoint.USEast1))
            {
                var req = new SendEmailRequest()
                {
                    Source = email.From.Address,
                    Destination = new Destination()
                    {
                        ToAddresses = email.To.Select(e => e.Address).ToList()
                    },
                    Message = ReformatToAWSMessage(email),
                };

                try
                {
                    var response = await client.SendEmailAsync(req);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        private Message ReformatToAWSMessage(MailMessage email)
        {
            var res = new Message();
            res.Subject = new Content(email.Subject);
            res.Body = new Body
            {
                Html = new Content
                {
                    Charset = "UTF-8",
                    Data = email.Body
                }
            };

            return res;

        }
    }
}
