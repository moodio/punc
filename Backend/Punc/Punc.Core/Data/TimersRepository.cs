using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Punc.Data
{
    internal class TimersRepository : ITimersRepository
    {
        private readonly string _connectionString;

        public TimersRepository(IConfiguration config)
        {
            _connectionString = config["Database:ConnectionString"];
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<Timer> CreateTimerAsync(Timer timer)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    var dbname = await conn.QuerySingleAsync<string>("SELECT current_database()");

                    var res = await conn.QuerySingleAsync<TimerDataModel>("create_timer", (TimerDataModel) timer,
                        commandType: CommandType.StoredProcedure);

                    return (Timer) res;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Timer> GetTimerAsync(Guid id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<TimerDataModel>("SELECT * FROM timers WHERE id = @Id",
                    new {Id = id});
                return (Timer) res;
            }
        }

        public async Task<bool> UpdateTimerAsync(Timer timer)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                var res = await conn.QuerySingleAsync<TimerDataModel>("update_timer", (TimerDataModel)timer,
                    commandType: CommandType.StoredProcedure);

                return true;
            }
        }
    }
}
