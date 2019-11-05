using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IRefereeCodesService
    {
        
        /// <summary>
        /// Create a new confirmation code
        /// </summary>
        /// <param name="timerId"></param>
        /// <returns></returns>
        Task<string> CreateRefereeCodeAsync(Guid timerId);


//        /// <summary>
//        /// confirm the validity of a confirmation code
//        /// </summary>
//        /// <param name="timerId"></param>
//        /// <param name="refereeCode"></param>
//        /// <returns></returns>
//        Task<bool> ValidateRefereeCodeAsync(Guid timerId, string refereeCode);

        Task<Guid?> GetRefereeTimerIdAsync(string refereeCode);
    }
}
