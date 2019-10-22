using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface IAutoCompleteService
    {
        Task<List<string>> Search(string sessionKey, string query, string location = null); 
    }
}
