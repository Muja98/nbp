using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4jClient;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using StackExchange.Redis;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.DTOs;
using Neo4jClient.Cypher;

namespace Share_To_Learn_WEB_API.Services.RepositoryServices
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;

        public GroupRepository(IGraphClient client, IRedisConnectionBuilder builder)
        {
            _client = client;
            _redisConnection = builder.Connection;
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

        public async Task DeleteGroup(int groupId)
        {
            await _client.Cypher
                 .Match("(gr:Group),(ps:Post),(st:Student),(co:Comment)," +
                 "(st)-[:WROTECOMMENT]->(co),(co)-[:STOREDIN]->(ps),(ps) -[:BELONG]->(gr)")
                 .Where("ID(gr) = $groupId")
                 .WithParam("groupId", groupId)
                 .DetachDelete("co")
                 .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .Match("(gr:Group),(ps:Post),(ps)-[:BELONG]->(gr)")
                .Where("ID(gr) = $groupId")
                .WithParam("groupId", groupId)
                .DetachDelete("ps")
                .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .Match(" (gr:Group), (dc:Document), (gr)-[:CONTAINS]->(dc)")
                .Where("ID(gr) = $groupId")
                .WithParam("groupId", groupId)
                .DetachDelete("dc")
                .ExecuteWithoutResultsAsync();

            await _client.Cypher
                .Match(" (gr:Group)")
                .Where("ID(gr) = $groupId")
                .WithParam("groupId", groupId)
                .DetachDelete("gr")
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

            var countOfComments = await _client.Cypher
                .Match("(c:Comment)-[:STOREDIN]->(p:Post)-[:BELONG]->(g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct c)")
                .ResultsAsync;

            var countOfMembers = await _client.Cypher
                .Match("(st:Student)-[rel]-> (g:Group)")
                .Where("ID(g) = $groupId")
                .WithParam("groupId", groupId)
                .Return<int>("count(distinct st)")
                .ResultsAsync;

            group.Single().CountOfMembers = countOfMembers.Single();
            group.Single().CountOfPosts = countOfPosts.Single();
            group.Single().CountOfComments = countOfComments.Single();

            return group.Single();

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

    }
}
