using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc.Data
{
    public interface IRefereeCodesRepository
    {
        Task<bool> CreateRefereeCodeAsync(string code, Guid timerId);

        Task<Guid?> GetRefereeCodeTimerId(string code);
    }
}
