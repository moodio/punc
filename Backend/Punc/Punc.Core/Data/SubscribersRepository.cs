using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Punc.Data
{
    internal class SubscribersRepository : ISubscribersRepository
    {
        private readonly string _connectionString;

        public SubscribersRepository(IConfiguration config)
        {
            _connectionString = config["Database:ConnectionString"];
        }


        public async Task<bool> Subscribe(string email)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<bool>("subscribe", new {Email = email},
                    commandType: CommandType.StoredProcedure);
                return res;
            }
        }

        public async Task<bool> Unsubscribe(string email)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<bool>("unsubscribe", new {Email = email},
                    commandType: CommandType.StoredProcedure);
                return res;
            }
        }
    }
}
