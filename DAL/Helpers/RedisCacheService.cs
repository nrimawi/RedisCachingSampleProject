using StackExchange.Redis;
using System.Threading.Tasks;

namespace DAL.Helpers
{
    public class RedisCacheService : ICashServices
    {

        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }
        public async Task<string> GetCachValueAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task SetCachValueAsync(string key, string value)
        {
            var db = _connectionMultiplexer.GetDatabase();

            await db.StringSetAsync(key, value,System.TimeSpan.FromHours(1));
        }
    }
}
