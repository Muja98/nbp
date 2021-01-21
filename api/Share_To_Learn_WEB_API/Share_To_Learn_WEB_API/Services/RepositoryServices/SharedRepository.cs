using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using StackExchange.Redis;

namespace Share_To_Learn_WEB_API.Services.RepositoryServices
{
    public class SharedRepository : ISharedRepository
    {
        private readonly IConnectionMultiplexer _redisConnection;
        public SharedRepository(IRedisConnectionBuilder builder)
        {
            _redisConnection = builder.Connection;
        }

        public async Task<string> GetNextDocumentId()
        {
            IDatabase db = _redisConnection.GetDatabase();
            long result = await db.StringIncrementAsync("next.document.id");

            return result.ToString();
        }

        public async Task<string> GetNextImageId()
        {
            IDatabase db = _redisConnection.GetDatabase();
            long result = await db.StringIncrementAsync("next.image.id");

            return result.ToString();
        }
    }
}
