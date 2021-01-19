using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.DTOs;
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
                            var redisPubSub = _connection.GetSubscriber();
                            redisPubSub.Subscribe("friend.requests").OnMessage(message =>
                            {
                                Message deserializedMessage = JsonSerializer.Deserialize<Message>(message.Message);
                                int biggerId = deserializedMessage.SenderId > deserializedMessage.ReceiverId ? deserializedMessage.SenderId : deserializedMessage.ReceiverId;
                                int smallerId = deserializedMessage.SenderId < deserializedMessage.ReceiverId ? deserializedMessage.SenderId : deserializedMessage.ReceiverId;
                                string groupName = $"messages:{biggerId}:{smallerId}:chat";
                                _ = _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", deserializedMessage);
                            });


                            var subPatternChannel = new RedisChannel("__keyevent@0__:*", RedisChannel.PatternMode.Pattern);
                            redisPubSub.Subscribe(subPatternChannel).OnMessage(message =>
                            {
                                string str = message.Channel;
                                if(str == "__keyevent@0__:expired" || str == "__keyevent@0__:del")
                                {
                                    string keyName = message.Message;
                                    string[] keyNameParts = keyName.Split(':');
                                    if(keyNameParts.Length==4 && keyNameParts[0]=="messages" && keyNameParts[3]=="chat")
                                    {
                                        int biggerId = int.Parse(keyNameParts[1]), smallerId = int.Parse(keyNameParts[2]);
                                        string setKeyBigger = $"student:{biggerId}:chats";
                                        string setKeySmaller = $"student:{smallerId}:chats";
                                        IDatabase redisDB = _connection.GetDatabase();
                                        var setEntriesBigger = redisDB.SortedSetRangeByRank(setKeyBigger, 0, -1, Order.Descending);
                                        var setEntriesSmaller = redisDB.SortedSetRangeByRank(setKeySmaller, 0, -1, Order.Descending);

                                        foreach (var entry in setEntriesBigger)
                                        {
                                            StudentDTO student = JsonSerializer.Deserialize<StudentDTO>(entry);
                                            if (student.Id == smallerId)
                                            {
                                                redisDB.SortedSetRemove(setKeyBigger, JsonSerializer.Serialize(student));
                                                break;
                                            }
                                        }

                                        foreach (var entry in setEntriesSmaller)
                                        {
                                            StudentDTO student = JsonSerializer.Deserialize<StudentDTO>(entry);
                                            if (student.Id == biggerId)
                                            {
                                                redisDB.SortedSetRemove(setKeySmaller, JsonSerializer.Serialize(student));
                                                break;
                                            }
                                        }
                                    }
                                    
                                }
                                
                            });
                        }
                    }
                }
                return _connection;
            }
        }
    }
}

