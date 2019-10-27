using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc.Data
{
    public interface ITimersRepository
    {
        Task<Timer> CreateTimerAsync(Timer timer);

        Task<Timer> GetTimerAsync(Guid id);

        Task<bool> UpdateTimerAsync(Timer timer);
    }
}
