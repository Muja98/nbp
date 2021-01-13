using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Share_To_Learn_WEB_API.RedisConnection
{
    public class RedisConnectionBuilder : IRedisConnectionBuilder
    {
        private static IConnectionMultiplexer _connection = null;
        private static object _objectLock = new object();
        private static readonly string ConnectionString = "localhost:2055";

        public IConnectionMultiplexer Connection
        {
            get
            {
                if(_connection == null)
                {
                    lock (_objectLock)
                    {
                        if (_connection == null)
                            _connection = ConnectionMultiplexer.Connect(ConnectionString);
                    }
                }
                return _connection;
            }
        }
    }
}
