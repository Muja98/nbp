using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Services.RepositoryServices
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IConnectionMultiplexer _redisConnection;
        //private readonly IHubContext<MessageHub> _hub;
        public MessageRepository(IRedisConnectionBuilder builder)
        {
            _redisConnection = builder.Connection;
        }
        public async Task SendMessage(Message message)
        {
            var values = new NameValueEntry[]
            {
                new NameValueEntry("sender", message.Sender),
                new NameValueEntry("senderId", message.SenderId),
                new NameValueEntry("receiver", message.Receiver),
                new NameValueEntry("receiverId", message.ReceiverId),
                new NameValueEntry("content", message.Content)
            };
            IDatabase redisDB = _redisConnection.GetDatabase();
            int biggerId = message.SenderId > message.ReceiverId ? message.SenderId : message.ReceiverId;
            int smallerId = message.SenderId < message.ReceiverId ? message.SenderId : message.ReceiverId;
            await redisDB.StreamAddAsync($"messages:{biggerId}:{smallerId}:chat", values);

            var jsonMessage = JsonSerializer.Serialize(message);
            ISubscriber chatPubSub = _redisConnection.GetSubscriber();
            await chatPubSub.PublishAsync("chat.messages", jsonMessage);
        }

        public async Task<IEnumerable<MessageDTO>> ReceiveMessage(int senderId, int receiverId, string from, int count)
        {
            List<MessageDTO> retMessages = new List<MessageDTO>();
            int biggerId = senderId > receiverId ? senderId : receiverId;
            int smallerId = senderId < receiverId ? senderId : receiverId;
            string channelName = $"messages:{biggerId}:{smallerId}:chat";
            IDatabase redisDb = _redisConnection.GetDatabase();
            from = Uri.UnescapeDataString(from);
            var messages = await redisDb.StreamRangeAsync(channelName, minId: "-", maxId: from, count: count, messageOrder: Order.Descending);
            foreach (var message in messages)
            {
                MessageDTO mess = new MessageDTO
                {
                    Id = message.Id,
                    Sender = message.Values.FirstOrDefault(value => value.Name == "sender").Value,
                    SenderId = int.Parse(message.Values.FirstOrDefault(value => value.Name == "senderId").Value),
                    Receiver = message.Values.FirstOrDefault(value => value.Name == "receiver").Value,
                    ReceiverId = int.Parse(message.Values.FirstOrDefault(value => value.Name == "receiverId").Value),
                    Content = message.Values.FirstOrDefault(value => value.Name == "content").Value
                };
                retMessages.Add(mess);
            }
            return retMessages;
        }

        public async Task StartConversation(ConversationDTO participants)
        {
            string senderSetKey = $"student:{participants.Sender.Id}:chats";
            string receiverSetKey = $"student:{participants.Receiver.Id}:chats";
            Student sender = participants.Sender.Student;
            Student receiver = participants.Receiver.Student;

            var senderSetValue = JsonSerializer.Serialize(participants.Receiver);
            var receiverSetValue = JsonSerializer.Serialize(participants.Sender);

            double score = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            SortedSetEntry[] senderSetEntry = new SortedSetEntry[] { new SortedSetEntry(senderSetValue, score) };
            SortedSetEntry[] receiverSetEntry = new SortedSetEntry[] { new SortedSetEntry(receiverSetValue, score) };

            IDatabase redisDB = _redisConnection.GetDatabase();
            await redisDB.SortedSetAddAsync(senderSetKey, senderSetEntry);
            await redisDB.SortedSetAddAsync(receiverSetKey, receiverSetEntry);
        }

        public async Task SetTimeToLiveForStream(int senderId, int receiverId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();

            int biggerId = senderId > receiverId ? senderId : receiverId;
            int smallerId = senderId < receiverId ? senderId : receiverId;

            await redisDB.KeyExpireAsync($"messages:{biggerId}:{smallerId}:chat", new TimeSpan(48, 0, 0));
        }

        public async Task<int> GetTimeToLiveForStream(int senderId, int receiverId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();

            int biggerId = senderId > receiverId ? senderId : receiverId;
            int smallerId = senderId < receiverId ? senderId : receiverId;
            TimeSpan? timeToLive = await redisDB.KeyTimeToLiveAsync($"messages:{biggerId}:{smallerId}:chat");
            return (int)timeToLive.Value.TotalSeconds;
        }

        public async Task<IEnumerable<StudentDTO>> GetStudentsInChatWith(int studentId)
        {
            List<StudentDTO> students = new List<StudentDTO>();
            string setKey = $"student:{studentId}:chats";
            IDatabase redisDB = _redisConnection.GetDatabase();
            var studentSetEntries = await redisDB.SortedSetRangeByRankAsync(setKey, 0, -1, Order.Descending);
            foreach (var entry in studentSetEntries)
            {
                StudentDTO student = JsonSerializer.Deserialize<StudentDTO>(entry);
                students.Add(student);
            }
            return students;
        }

        public async Task<IEnumerable<int>> GetIdsStudentsInChatWith(int studentId)
        {
            List<int> studentIds = new List<int>();
            string setKey = $"student:{studentId}:chats";
            IDatabase redisDB = _redisConnection.GetDatabase();
            var studentSetEntries = await redisDB.SortedSetRangeByRankAsync(setKey, 0, -1, Order.Descending);
            foreach (var entry in studentSetEntries)
            {
                StudentDTO student = JsonSerializer.Deserialize<StudentDTO>(entry);
                studentIds.Add(student.Id);
            }
            return studentIds;
        }

        public async Task DeleteConversation(int biggerId, int smallerId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();
            string key = "messages:" + biggerId + ":" + smallerId + ":chat";
            await redisDB.KeyDeleteAsync(key);
        }
    }
}
