using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Punc
{
    public interface ISubscribersService
    {
        Task<bool> Subscribe(string email);

        Task<bool> Unsubscribe(string email);
    }
}
