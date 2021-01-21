using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Services.RepositoryServices;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.HubConfig;
using StackExchange.Redis;
using Neo4jClient.Cypher;
using Share_To_Learn_WEB_API.RedisConnection;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public class StudentRepository:IStudentRepository
    {

        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IHubContext<MessageHub> _hub;
        public StudentRepository(IGraphClient client, IRedisConnectionBuilder builder, IHubContext<MessageHub> hub)
        {
            _client = client;
            _redisConnection = builder.Connection;
            _hub = hub;
        }


        public async Task<IEnumerable<StudentDTO>> GetStudentsPage(string filter, string userFilter, string orderBy, bool descending, int from, int to, int user)
        {
            var a = _client.Cypher
                        .Match($"(s1:Student), (s2:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            ICypherFluentQuery<StudentDTO> ret = a.Return(() => new StudentDTO
            {
                Id = Return.As<int>("ID(s1)"),
                IsFriend = Return.As<bool>("exists((s1)-[:FRIEND]-(s2))"),
                Student = Return.As<Student>("s1")
            });

            ret = ret.OrderBy(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }

        public async Task<string> getNextId(bool isImage)
        {
            IDatabase db = _redisConnection.GetDatabase();
            long result = 0;

            if (isImage)
                result = await db.StringIncrementAsync("next.image.id");
            else
                result = await db.StringIncrementAsync("next.document.id");

            return result.ToString();
        }

        public async Task<bool> CreateNonExistingStudent(StudentRegisterDTO newStudent)
        {
            Student rawStudent = newStudent.Student;
            string password = newStudent.Password;
            var res = await _client.Cypher
                            .Merge("(student:Student {Email: $email})")
                            .WithParam("email", rawStudent.Email)
                            .OnCreate().Set("student = $rawStudent, student.IsNew = true, student.Password = $password")
                            .WithParams(new
                            {
                                rawStudent = rawStudent,
                                password = password
                            })
                            .OnMatch().Set("student.IsNew = false")
                            .Return<bool>("student.IsNew")
                            .ResultsAsync;

            return res.Single();
        }

        public async Task<bool> StudentExists(int studentId)
        {
            var res = await _client.Cypher
                .Match("(student:Student)")
                .Where("ID(student) = $studentId")
                .WithParam("studentId", studentId)
                .Return<int>("count(student)")
                .ResultsAsync;

            return res.Single() > 0;
        }

        public async Task<StudentDTO> StudentExists(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return(() => new StudentDTO
                            {
                                Id = Return.As<int>("ID(student)"),
                                Student = Return.As<Student>("student")
                            }).ResultsAsync;

            return res.Single();
        }

        public async Task<string> GetPassword(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return<string>("student.Password")
                            .ResultsAsync;

            if (res.Count() > 0)
                return res.Single();
            else
                return null;
        }

        public async Task UpdateStudent(int studentId, Student updatedStudent)
        {
            await _client.Cypher
                .Match("(student: Student)")
                .Where("ID(student) = $studentId")
                .WithParam("studentId", studentId)
                .Set("student.FirstName = $FirstName")
                .WithParam("FirstName", updatedStudent.FirstName)
                .Set("student.LastName = $LastName")
                .WithParam("LastName", updatedStudent.LastName)
                .Set("student.DateOfBirth = $DateOfBirth")
                .WithParam("DateOfBirth", updatedStudent.DateOfBirth)
                .Set("student.ProfilePicturePath = $ProfilePicturePath")
                .WithParam("ProfilePicturePath", updatedStudent.ProfilePicturePath)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<int> GetStudentsCount(string filter, string userFilter)
        {
            var a = _client.Cypher
                        .Match($"(s1:Student), (s2:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            var res = await a.Return<int>("count(s1)").ResultsAsync;
            return res.Single();
        }

        public async Task DeleteFriendRequest(int receiverId, string requestId, int senderId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            IDatabase redisDB = _redisConnection.GetDatabase();
            long deletedMessages = await redisDB.StreamDeleteAsync(channelName, new RedisValue[] { new RedisValue(requestId) });
            await redisDB.SetRemoveAsync("friend:" + senderId + ":request", receiverId);
        }

        public async Task AddFriend(int studentId1, int studentId2)
        {
            await _client.Cypher
                   .Match("(st1:Student), (st2:Student)")
                   .Where("ID(st1) = $studentId1")
                   .WithParam("studentId1", studentId1)
                   .AndWhere("ID(st2) = $studentId2")
                   .WithParam("studentId2", studentId2)
                   .Create("(st1)-[:FRIEND]->(st2)")
                   .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveFriend(int studentId1, int studentId2)
        {
            await _client.Cypher
                   .Match("(st1:Student)-[f1:FRIEND]-(st2:Student)")
                   .Where("ID(st1) = $studentId1")
                   .WithParam("studentId1", studentId1)
                   .AndWhere("ID(st2) = $studentId2")
                   .WithParam("studentId2", studentId2)
                   .Delete("f1")
                   .ExecuteWithoutResultsAsync();
        }
        public async Task<IEnumerable<StudentDTO>> GetFriendsPage(string filter, string userFilter, string orderBy, bool descending, int from, int to)
        {
            var a = _client.Cypher
                        .Match("(s:Student)-[:FRIEND]-(s1:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            ICypherFluentQuery<StudentDTO> ret = a.Return(() => new StudentDTO
            {
                Id = Return.As<int>("ID(s1)"),
                IsFriend = Return.As<bool>("true"),
                Student = Return.As<Student>("s1")
            });


            ret = ret.OrderBy(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }

        public async Task<int> GetFriendsCount(string filter, int userId)
        {
            var a = _client.Cypher
            .Match("(st1: Student), (s:Student), (st1)-[r:FRIEND]-(s)")
            .Where("ID(st1) = $userId")
            .WithParam("userId", userId);

            if (!string.IsNullOrEmpty(filter))
                a = a.AndWhere(filter);

            var res = await a.Return<int>("count(distinct s)").ResultsAsync;
            return res.Single();
        }

        public async Task<StudentDTO> GetSpecificStudent(int studentId, int requesterId)
        {
            var student = await _client.Cypher
                    .Match("(s:Student), (s1:Student)")
                    .Where("ID(s) = $studentId AND ID(s1) = $requesterId")
                    .WithParams(new
                    {
                        studentId = studentId,
                        requesterId = requesterId
                    })
                    .Return(() => new StudentDTO
                    {
                        Id = Return.As<int>("ID(s)"),
                        IsFriend = Return.As<bool>("exists((s)-[:FRIEND]-(s1))"),
                        Student = Return.As<Student>("s")
                    }).ResultsAsync;
            return student.Single();
        }

        public async Task SendFriendRequest(int senderId, int receiverId, Request sender)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            var values = new NameValueEntry[]
            {
                new NameValueEntry("sender_id", senderId),
                new NameValueEntry("sender_first_name", sender.FirstName),
                new NameValueEntry("sender_last_name", sender.LastName),
                new NameValueEntry("sender_email", sender.Email),
                new NameValueEntry("sender_profile_picture_path", sender.ProfilePicturePath),

            };

            IDatabase redisDB = _redisConnection.GetDatabase();
            var messageId = await redisDB.StreamAddAsync(channelName, values);

            //Dodatna funkcija
            await redisDB.SetAddAsync("friend:" + senderId + ":request", receiverId);

            // objekat za notifikaciju
            FriendRequestNotificationDTO message = new FriendRequestNotificationDTO
            {
                ReceiverId = receiverId,
                RequestDTO = new RequestDTO { Id = messageId, Request = sender }
            };

            //Push notifikacija
            var jsonMessage = JsonSerializer.Serialize(message);
            ISubscriber chatPubSub = _redisConnection.GetSubscriber();
            await chatPubSub.PublishAsync("friendship.requests", jsonMessage);
        }

        public async Task<IEnumerable<RequestDTO>> GetFriendRequests(int receiverId)
        {
            string channelName = $"messages:{receiverId}:friend_request";
            IDatabase redisDB = _redisConnection.GetDatabase();

            var requests = await redisDB.StreamReadAsync(channelName, "0-0");

            IList<RequestDTO> result = new List<RequestDTO>();

            foreach (var request in requests)
            {
                result.Add(
                    new RequestDTO
                    {
                        Id = request.Id,
                        Request = new Request
                        {
                            Id = int.Parse(request.Values.FirstOrDefault(value => value.Name == "sender_id").Value),
                            FirstName = request.Values.FirstOrDefault(value => value.Name == "sender_first_name").Value,
                            LastName = request.Values.FirstOrDefault(value => value.Name == "sender_last_name").Value,
                            Email = request.Values.FirstOrDefault(value => value.Name == "sender_email").Value,
                            ProfilePicturePath = request.Values.FirstOrDefault(value => value.Name == "sender_profile_picture_path").Value
                        }

                    }
                );
            }

            return result;
        }

        public async Task<IEnumerable<int>> GetFriendRequestSends(int senderId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();
            var result = await redisDB.SetMembersAsync("friend:" + senderId + ":request");

            IList<int> arr = new List<int>();

            foreach (var res in result)
                arr.Add(Convert.ToInt32(res));

            return arr;
        }


    }
}
