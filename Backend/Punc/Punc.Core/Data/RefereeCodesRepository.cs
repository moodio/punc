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
    internal class RefereeCodesRepository : IRefereeCodesRepository
    {
        private readonly string _connectionString;

        public RefereeCodesRepository(IConfiguration config)
        {
            _connectionString = config["Database:ConnectionString"];
        }

        public async Task<bool> CreateRefereeCodeAsync(string code, Guid timerId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<bool>("create_referee_code", 
                    new {Code = code, TimerId = timerId}, commandType: CommandType.StoredProcedure);
                return res;
            }
        }

        public async Task<Guid?> GetRefereeCodeTimerId(string code)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<Guid?>("get_referee_timer_id", new {Code = code},
                    commandType: CommandType.StoredProcedure);
                return res;
            }
        }

    }
}
