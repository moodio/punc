using Punc.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface ITimersService
    {
        Task<Timer> CreateTimerAsync(CreateTimerRequest req);

        Task<Timer> GetTimerAsync(Guid id);
    }
}
