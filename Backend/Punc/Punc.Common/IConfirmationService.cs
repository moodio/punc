using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IConfirmationService
    {
        //send referee email
        Task<bool> SendRefereeEmailAsync(Timer timer);

        //confirm punctuality
        Task<bool> ConfirmPunctualityAsync(Guid timerId, string confirmationCode, bool ontime);

        Task<string> CreateConfirmationCodeAsync(Guid timerId);
    }
}
