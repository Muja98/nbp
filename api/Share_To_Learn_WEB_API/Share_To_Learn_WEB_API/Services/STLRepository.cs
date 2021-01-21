using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;
using Neo4jClient.Cypher;
using StackExchange.Redis;
using Share_To_Learn_WEB_API.RedisConnection;
using Microsoft.AspNetCore.SignalR;
using Share_To_Learn_WEB_API.HubConfig;
using System.Text.Json;
using System.Text;

namespace Share_To_Learn_WEB_API.Services
{
    public class STLRepository : ISTLRepository
    {  
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IHubContext<MessageHub> _hub;
        public STLRepository(IGraphClient client, IRedisConnectionBuilder builder, IHubContext<MessageHub> hub)
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

        public async Task CreateStudent(Student newStudent)
        {
            await _client.Cypher
                .Create("(student:Student $newStudent)")
                .WithParam("newStudent", newStudent)
                .ExecuteWithoutResultsAsync();
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

        public async Task CreateGroup(int ownerId, Group newGroup)
        {
            await _client.Cypher
                    .Match("(owner: Student)")
                    .Where("ID(owner) = $ownerId")
                    .WithParam("ownerId", ownerId)
                    .Create("(owner)-[:OWNER]->(group:Group $newGroup)")
                    .WithParam("newGroup", newGroup)
                    .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId)
        {
            var res = await _client.Cypher

                            .Match("(st:Student)-[MEMBER]-(gr:Group)")
                            .Where("ID(st) = $studentId")
                            .WithParam("studentId", studentId)
                            .AndWhere("ID(gr) = $groupId")
                            .WithParam("groupId", groupId)
                            .Return(st => st.As<Student>())
                            .ResultsAsync;
            return res.Count() == 0;
        }

        public async Task AddStudentToGroup(int studentId, int groupId)
        {
            await _client.Cypher
                             .Match("(st:Student),(gr:Group)")
                             .Where("ID(st) = $studentId")
                             .WithParam("studentId", studentId)
                             .AndWhere("ID(gr) = $groupId")
                             .WithParam("groupId", groupId)
                             .Create("(st)-[:MEMBER]->(gr)")
                             .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveStudentFromGroup(int studentId, int groupId)
        {
            await _client.Cypher
                            .Match("(st:Student),(gr:Group)")
                            .Match("(st)-[r:MEMBER]-(gr)")
                            .Where("ID(st) = $studentId")
                            .WithParam("studentId", studentId)
                            .AndWhere("ID(gr) = $groupId")
                            .WithParam("groupId", groupId)
                            .Delete("r")
                            .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateGroup(int groupId, Group updatedGroup)
        {
            await _client.Cypher
               .Match("(group: Group)")
               .Where("ID(group) = $groupId")
               .WithParam("groupId", groupId)
               .Set("group.Name = $Name")
               .WithParam("Name", updatedGroup.Name)
               .Set("group.Field = $Field")
               .WithParam("Field", updatedGroup.Field)
               .Set("group.Description = $Description")
               .WithParam("Description", updatedGroup.Description)
               .Set("group.GroupPicturePath = $GroupPicturePath")
               .WithParam("GroupPicturePath", updatedGroup.GroupPicturePath)
               .ExecuteWithoutResultsAsync();

               
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

        public async Task<bool> GroupExists(int groupId)
        {
            var res = await _client.Cypher
                .Match("(group: Group)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(group)")
                .ResultsAsync;

            return res.Single() > 0;
        }

        public async Task<IEnumerable<GroupDTO>> GetGroupsPage(string filters, string userFilter, string orderBy, bool descending, int from, int to)
        {
            var a = _client.Cypher
                        .Match("(g:Group), (s:Student), (ow)-[:OWNER]-(g)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            a = a.With("g, {Id: ID(ow), Student: ow} as st");

            ICypherFluentQuery<GroupDTO> ret = a.Return((g, st) => new GroupDTO
            {
                Id = Return.As<int>("ID(g)"),
                Group = Return.As<Group>("g"),
                Student = st.As<StudentDTO>()
            });

            if (descending)
                ret = ret.OrderByDescending(orderBy);
            else
                ret = ret.OrderBy(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }


        public async Task<IEnumerable<GroupDTO>> GetMemberships(int studentId)
        {
            var result = await _client.Cypher
                            .Match("(s)-[r:MEMBER]-(g), (ow)-[rel:OWNER]-(g)")
                            .Where("ID(s) = $studentId")
                            .WithParam("studentId", studentId)
                            .With("g, {Id: ID(ow), Student: ow} as st")
                            .Return((g, st) => new GroupDTO
                            {
                                Id = Return.As<int>("ID(g)"),
                                Group = Return.As<Group>("g"),
                                Student = st.As<StudentDTO>()
                            })
                            .ResultsAsync;

            return result;
        }

        public async Task<IEnumerable<GroupDTO>> GetOwnerships(int studentId)
        {
            var result = await _client.Cypher
                .Match("(ow)-[r:OWNER]-(g)")
                .Where("ID(ow) = $studentId")
                .WithParam("studentId", studentId)
                .With("g, {Id: ID(ow), Student: ow} as st")
                .Return((g, st) => new GroupDTO
                {
                    Id = Return.As<int>("ID(g)"),
                    Group = Return.As<Group>("g"),
                    Student = st.As<StudentDTO>()
                })
                .ResultsAsync;

            return result;
        }
      

        public async Task<int> GetGroupsCount(string filters, string userFilter)
        {
            var a = _client.Cypher
                        .Match("(g:Group), (s:Student), (ow)-[:OWNER]-(g)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            var res = await a.Return<int>("count(g)").ResultsAsync;
            return res.Single();
        }

        public async Task<IEnumerable<PostDTO>> GetAllPosts(int groupId)
        {
            var result = await _client.Cypher
                         .Match("(gr:Group),(ps:Post), (st:Student), (ps)-[:BELONG]-(gr), (st)-[:WRITE]-(ps)")
                         .Where("ID(gr) = $groupId")
                         .WithParam("groupId", groupId)
                         .With("ps,{Id: ID(st), Student: st} as pst")
                         .Return((gr, pst) => new PostDTO
                         {
                             Id = Return.As<int>("ID(ps)"),
                             Post = Return.As<Post>("ps"),
                             Student = pst.As<StudentDTO>()
                         })
                        .ResultsAsync;
            return result;
        }

        public async Task <PostDTO> CreatePost(int groupId, int studentId, Post newPost)
        {
            IEnumerable<PostDTO> p =  await _client.Cypher
            .Match("(st:Student), (gr:Group)")
            .Where("ID(gr) = $groupId")
            .WithParam("groupId", groupId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WRITE]->(ps:Post $newPost)")
            .WithParam("newPost",newPost)
            .Create("(ps)-[:BELONG]->(gr)")
            .With("ps,{Id: ID(st), Student: st} as pst")
            .Return((gr, pst) => new PostDTO
            {
                Id = Return.As<int>("ID(ps)"),
                Post = Return.As<Post>("ps"),
                Student = pst.As<StudentDTO>()
            })
            .ResultsAsync;
            PostDTO g = p.Single();

            return g;
        }
        
        public async Task<IEnumerable<CommentDTO>> GetAllComment(int postId)
        {
            var result = await _client.Cypher
                         .Match("(co:Comment),(ps:Post), (st:Student), (co)-[:STOREDIN]-(ps), (st)-[:WROTECOMMENT]-(co)")
                         .Where("ID(ps) = $postId")
                         .WithParam("postId", postId)
                         .With("co, {Id: ID(st), Student: st} as pst")
                         .Return((co, pst) => new CommentDTO
                         {
                             Id = Return.As<int>("ID(co)"),
                             Comment = Return.As<Comment>("co"),
                             Student = pst.As<StudentDTO>()
                         })
                        .ResultsAsync;
            return result;
        }

        public async Task<CommentDTO> CreateComment(int postId, int studentId, Comment newComment)
        {
            IEnumerable<CommentDTO> c =  await _client.Cypher
            .Match("(st:Student), (ps:Post)")
            .Where("ID(ps) = $postId")
            .WithParam("postId", postId)
            .AndWhere("ID(st) = $studentId")
            .WithParam("studentId", studentId)
            .Create("(st)-[:WROTECOMMENT]->(co:Comment $newComment)")
            .WithParam("newComment", newComment)
            .Create("(co)-[:STOREDIN]->(ps)")
            .With("co, {Id: ID(st), Student: st} as pst")
            .Return((co, pst) => new CommentDTO
            {
                Id = Return.As<int>("ID(co)"),
                Comment = Return.As<Comment>("co"),
                Student = pst.As<StudentDTO>()
            })
            .ResultsAsync;

            CommentDTO g = c.Single();

            return g;
        }

        public async Task DeleteComment(int commentId)
        {
            await _client.Cypher
                .Match("(co:Comment)")
                .Where("ID(co) = $commentId")
                .WithParam("commentId", commentId)
                .DetachDelete("co")
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateComment(int commentId, Comment comment)
        {
               await _client.Cypher
                    .Match("(co:Comment)")
                    .Where("ID(co) = $commentId")
                    .WithParam("commentId", commentId)
                    .Set("co.Content = $comment")
                    .WithParam("comment", comment.Content)
                    .ExecuteWithoutResultsAsync();
        }

        public async Task UpdatePost(int postId, Post post)
        {
            await _client.Cypher
                   .Match("(ps:Post)")
                   .Where("ID(ps) = $postId")
                   .WithParam("postId", postId)
                   .Set("ps.Content = $content")
                   .WithParam("content", post.Content)
                   .ExecuteWithoutResultsAsync();
        }

        public async Task DeletePost(int postId)
        {
            await _client.Cypher
                    .Match("(co: Comment), (ps: Post), (co)-[:STOREDIN]->(ps)")
                    .Where("ID(ps) = $postId")
                    .WithParam("postId", postId)
                    .DetachDelete("co, ps")
                    .ExecuteWithoutResultsAsync();

            await _client.Cypher
                    .Match("(ps: Post)")
                    .Where("ID(ps) = $postId")
                    .WithParam("postId", postId)
                    .DetachDelete("ps")
                    .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<StudentDTO>> GetGroupMembers(int groupId, int requesterId)
        {
            return await _client.Cypher
                    .Match("(s:Student)-[:MEMBER]-(g:Group), (s1:Student)")
                    .Where("ID(g) = $groupId AND ID(s1) = $requesterId")
                    .WithParams(new
                    {
                        groupId = groupId,
                        requesterId = requesterId
                    })
                    .Return(() => new StudentDTO
                    {
                        Id = Return.As<int>("ID(s)"),
                        IsFriend = Return.As<bool>("exists((s1)-[:FRIEND]-(s))"),
                        Student = Return.As<Student>("s")
                    }).ResultsAsync;
        }

        public async Task<StudentDTO> GetGroupOwner(int groupId, int requesterId)
        {
            var res = await _client.Cypher
                    .Match("(s:Student)-[:OWNER]-(g:Group), (s1:Student)")
                    .Where("ID(g) = $groupId AND ID(s1) = $requesterId")
                    .WithParams(new
                    {
                        groupId = groupId,
                        requesterId = requesterId
                    })
                    .Return(() => new StudentDTO
                    {
                        Id = Return.As<int>("ID(s)"),
                        IsFriend = Return.As<bool>("exists((s1)-[:FRIEND]-(s))"),
                        Student = Return.As<Student>("s")
                    }).ResultsAsync;

            return res.FirstOrDefault();
        }

        public async Task<GroupStatisticsDTO> GetGroupStatistics(int groupId)
        {
            var group = await _client.Cypher
                .Match("(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return(() => new GroupStatisticsDTO
                {
                    Group = Return.As<Group>("g"),
                }).ResultsAsync;

            var countOfPosts = await _client.Cypher
                .Match("(p:Post)-[:BELONG]->(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct p)")
                .ResultsAsync;

            var countOfComments= await _client.Cypher
                .Match("(c:Comment)-[:STOREDIN]->(p:Post)-[:BELONG]->(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct c)")
                .ResultsAsync;

            var countOfMembers = await _client.Cypher
                .Match("(st:Student)-[:Member]-> (g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct st)")
                .ResultsAsync;

            group.Single().CountOfMembers = countOfMembers.Single();
            group.Single().CountOfPosts = countOfPosts.Single();
            group.Single().CountOfComments = countOfComments.Single();

            return group.Single();

        }

        public async Task<string> GetGroupImage(int groupId)
        {
            var res = await _client.Cypher
                 .Match("(g: Group)")
                 .Where("ID(g) = $groupId")
                 .WithParam("groupId", groupId)
                 .Return<string>("g.GroupPicturePath")
                 .ResultsAsync;

            return res.Single();
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

        public async Task CreateDocument(int studentId, Document newDocument)
        {
            await _client.Cypher
                .Match("(creator: Student)")
                .Where("ID(creator) = $studentId")
                .WithParam("studentId", studentId)
                .Create("(creator)-[:CREATED]->(document:Document $newDocument)")
                .WithParam("newDocument", newDocument)
                .ExecuteWithoutResultsAsync();
        }

        public async Task RelateDocumentAndGroup(int groupId, string documentPath)
        {
            await _client.Cypher
                .Match("(group: Group), (document: Document)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId)
                .AndWhere("document.DocumentPath = $documentPath")
                .WithParam("documentPath", documentPath)
                .Create("(group)-[:CONTAINS]->(document)")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<IEnumerable<DocumentDTO>> GetDocuments(int groupId, string filter)
        {
            var process = _client.Cypher
                .Match("(group: Group)-[:CONTAINS]->(document: Document)")
                .Where("ID(group) = $groupId")
                .WithParam("groupId", groupId);

            if (!string.IsNullOrEmpty(filter))
                process = process.AndWhere(filter);

            var res = await process.Return(() => new DocumentDTO
            {
                Id = Return.As<int>("ID(document)"),
                Name = Return.As<string>("document.Name"),
                Level = Return.As<string>("document.Level"),
                Description = Return.As<string>("document.Description"),
            })
                .OrderBy("ID(document) desc")
                .ResultsAsync;

            return res;
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

        public async Task<string> GetDocumentsPath(int documentId)
        {
            var res = await _client.Cypher
                .Match("(document: Document)")
                .Where("ID(document) = $documentId")
                .WithParam("documentId", documentId)
                .Return<string>("document.DocumentPath")
                .ResultsAsync;

            return res.Single();
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

        public async Task<string> GetStudentGroupRelationship(int studentId, int groupId)
        {
            var res = await _client.Cypher
                      .Match("(s:Student)-[rel]-(g:Group)")
                      .Where("ID(s) = $studentId AND ID(g) = $groupId")
                      .WithParams(new
                      {
                          studentId = studentId,
                          groupId = groupId
                      })
                      .Return<string>("TYPE(rel)")
                      .ResultsAsync;
            return res.FirstOrDefault();
        }

        public async Task<IEnumerable<GroupDTO>> GetStudentGroups(int studentId)
        {
            var res = await _client.Cypher
                        .Match("(s:Student)-[rel]-(g:Group)")
                        .Where("ID(s) = $studentId")
                        .WithParam("studentId", studentId)
                        .With("g")
                        .Match("(ow:Student)-[:OWNER]-(g)")
                        .With("g, {Id: ID(g), Student: ow} as st")
                        .Return((g, st) => new GroupDTO
                        {
                            Id = Return.As<int>("ID(g)"),
                            Group = Return.As<Group>("g"),
                            Student = st.As<StudentDTO>()
                        }).ResultsAsync;

            return res;
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

            ////////////////

            var jsonMessage = JsonSerializer.Serialize(message);
            ISubscriber chatPubSub = _redisConnection.GetSubscriber();
            await chatPubSub.PublishAsync("chat.messages", jsonMessage);

            //string groupName = "peraIzika";
            //_ = _hub.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
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
            foreach(var message in messages)
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
        
        public async Task<string> getNextId(bool isImage)
        {
           IDatabase db =   _redisConnection.GetDatabase();
           long result = 0;

           if(isImage)
                result =  await db.StringIncrementAsync("next.image.id");    
           else
                result =  await db.StringIncrementAsync("next.document.id");

            return result.ToString(); 
        }

        public async Task DeleteFriendRequest(int receiverId, string requestId, int senderId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            IDatabase redisDB = _redisConnection.GetDatabase();
            long deletedMessages = await redisDB.StreamDeleteAsync(channelName, new RedisValue[] { new RedisValue(requestId) });
            await redisDB.SetRemoveAsync("friend:" + senderId + ":request", requestId);
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
                RequestDTO = new RequestDTO { Id = messageId, Request = sender}
            };

            //Push notifikacija
            var jsonMessage = JsonSerializer.Serialize(message);
            ISubscriber chatPubSub = _redisConnection.GetSubscriber();
            await chatPubSub.PublishAsync("friendship.requests", jsonMessage);
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

        public async Task StartConversation(ConversationDTO participants)
        {
            string senderSetKey = $"student:{participants.Sender.Id}:chats";
            string receiverSetKey = $"student:{participants.Receiver.Id}:chats";
            Student sender = participants.Sender.Student;
            Student receiver = participants.Receiver.Student;

            var senderSetValue = JsonSerializer.Serialize(participants.Receiver);
            var receiverSetValue = JsonSerializer.Serialize(participants.Sender);

            double score = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            SortedSetEntry[] senderSetEntry = new SortedSetEntry[]{ new SortedSetEntry(senderSetValue, score) };
            SortedSetEntry[] receiverSetEntry = new SortedSetEntry[] { new SortedSetEntry(receiverSetValue, score) };

            IDatabase redisDB = _redisConnection.GetDatabase();
            await redisDB.SortedSetAddAsync(senderSetKey, senderSetEntry);
            await redisDB.SortedSetAddAsync(receiverSetKey, receiverSetEntry);
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
        public async Task SetTimeToLiveForStream(int senderId, int receiverId)
        {
            IDatabase redisDB = _redisConnection.GetDatabase();

            int biggerId = senderId > receiverId ? senderId : receiverId;
            int smallerId = senderId < receiverId ? senderId : receiverId;

            await redisDB.KeyExpireAsync($"messages:{biggerId}:{smallerId}:chat", new TimeSpan(0, 2, 30));
        }

        public async Task DeleteGroup(int groupId)
        {
           await _client.Cypher
                .Match("(gr:Group),(ps:Post),(st:Student),(co:Comment)," +
                "(st)-[:WROTECOMMENT]->(co),(co)-[:STOREDIN]->(ps),(ps) -[:BELONG]->(gr)" )
                .Where("ID(gr) = $groupId")
                .WithParam("groupId", groupId)
                .DetachDelete("co")
                .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .Match(" (gr:Group),(ps:Post),(dc:Document)," +
                "(ps)-[:BELONG]->(gr),(gr) -[:CONTAINS]->(dc)")
                .Where("ID(gr) = $groupId")
                .WithParam("groupId", groupId)
                .DetachDelete("gr,ps,dc")
                .ExecuteWithoutResultsAsync();
        }
    }
}