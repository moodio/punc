using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Punc.Data;

namespace Punc
{
    internal class SubscribersService : ISubscribersService
    {
        private readonly ISubscribersRepository _repository;

        public SubscribersService(ISubscribersRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Subscribe(string email)
        {
            return await _repository.Subscribe(email);
        }

        public async Task<bool> Unsubscribe(string email)
        {
            return await _repository.Unsubscribe(email);
        }
    }
}
