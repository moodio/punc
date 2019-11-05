using Punc.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface ITimersService
    {
        Task<bool> ConfirmTimerOnTimeStatusAsync(string refereeCode, bool onTime);

        Task<Timer> CreateTimerAsync(CreateTimerRequest req);

        Task<RefereeTimer> GetRefereeTimerAsync(string refereeCode);

        Task<Timer> GetTimerAsync(Guid id);

    }
}
