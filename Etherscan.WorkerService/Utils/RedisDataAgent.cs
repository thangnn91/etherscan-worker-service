using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Etherscan.WorkerService.Utils
{
    public class RedisDataAgent : IRedisDataAgent
    {
        private static IDatabase _database;
        public RedisDataAgent()
        {
            var connection = RedisConnectionFactory.GetConnection();
            _database = connection.GetDatabase();
        }

        public string GetStringValue(string key)
        {
            return _database.StringGet(key);
        }

        public async Task<string> GetStringValueAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public void SetStringValue(string key, string value, int? expireSecond)
        {
            _ = expireSecond == null ? _database.StringSet(key, value) : _database.StringSet(key, value, new TimeSpan(0, 0, expireSecond.Value));
        }

        public async Task SetStringValueAsync(string key, string value, int? expireSecond)
        {
            _ = expireSecond == null ? await _database.StringSetAsync(key, value) : await _database.StringSetAsync(key, value, new TimeSpan(0, 0, expireSecond.Value));
        }

        public void DeleteStringValue(string key)
        {
            _database.KeyDelete(key);
        }
        public async Task DeleteStringValueAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}
