using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IGeocodingService
    {
        Task<string> ReverseLookup(decimal latitude, decimal longitude);
    }
}
