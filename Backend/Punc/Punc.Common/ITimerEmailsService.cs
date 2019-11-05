using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface ITimerEmailsService
    {
        /// <summary>
        /// Send referee email
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        Task<bool> SendRefereeEmailAsync(Timer timer);

    }
}
