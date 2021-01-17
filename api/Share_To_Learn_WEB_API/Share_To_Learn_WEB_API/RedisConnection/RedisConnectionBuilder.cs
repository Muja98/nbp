using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.HubConfig;
using StackExchange.Redis;

namespace Share_To_Learn_WEB_API.RedisConnection
{
    public class RedisConnectionBuilder : IRedisConnectionBuilder
    {
        private static IConnectionMultiplexer _connection = null;
        private static object _objectLock = new object();
        private static readonly string ConnectionString = "localhost:2055";
        private readonly IHubContext<MessageHub> _hub;

        public RedisConnectionBuilder(IHubContext<MessageHub> hub)
        {
            _hub = hub;
        }

        public IConnectionMultiplexer Connection
        {
            get
            {
                if(_connection == null)
                {
                    lock (_objectLock)
                    {
                        if (_connection == null)
                        { 
                            _connection = ConnectionMultiplexer.Connect(ConnectionString);
                            var friendRequestsPubSub = _connection.GetSubscriber();
                            friendRequestsPubSub.Subscribe("friend.requests").OnMessage(message =>
                            {
                                Message deserializedMessage = JsonSerializer.Deserialize<Message>(message.Message);
                                int biggerId = deserializedMessage.SenderId > deserializedMessage.ReceiverId ? deserializedMessage.SenderId : deserializedMessage.ReceiverId;
                                int smallerId = deserializedMessage.SenderId < deserializedMessage.ReceiverId ? deserializedMessage.SenderId : deserializedMessage.ReceiverId;
                                string groupName = $"messages:{biggerId}:{smallerId}:chat";
                                _ = _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", deserializedMessage);
                            });
                        }
                    }
                }
                return _connection;
            }
        }
    }
}
