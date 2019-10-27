using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IEmailService
    {
        Task<bool> SendEmail(MailMessage email);
    }
}
