using Container_App.Core.Interface.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Container_App.Service.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        public RedisService (IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task Delete(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<string?> Get(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task<T?> GetObject<T>(string key)
        {
            var json = await _database.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task Set(string key, string value)
        {
            await _database.StringSetAsync(key, value);
        }

        public async Task SetObject<T>(string key, T value, TimeSpan expiry)
        {
            var json = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(key, json, expiry);
        }
    }
}
