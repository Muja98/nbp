using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.RedisConnection
{
    public interface IRedisConnectionBuilder
    {
        IConnectionMultiplexer Connection { get; }
    }
}
