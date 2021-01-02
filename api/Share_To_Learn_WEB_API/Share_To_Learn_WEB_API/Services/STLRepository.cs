using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;
using Neo4jClient.Cypher;

namespace Share_To_Learn_WEB_API.Services
{
    public class STLRepository : ISTLRepository
    {
        private readonly IGraphClient _client;

        public STLRepository(IGraphClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Student>> GetStudents()
        {
            var res = await _client.Cypher
                        .Match(@"(student:Student)")
                        .Return(student => student.As<Student>
                     ()).ResultsAsync;

            return res;

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

        public async Task<bool> CreateNonExistingStudent(Student newStudent)
        {
            var res = await _client.Cypher
                            .Merge("(student:Student {Email: $email})")
                            .WithParam("email", newStudent.Email)
                            .OnCreate().Set("student = $newStudent, student.IsNew = true")
                            .WithParam("newStudent", newStudent)
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
                    .Create("(owner)-[:ADMINISTRATED]->(group:Group $newGroup)")
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
               .Set("group = $updatedGroup")
               .WithParam("updatedGroup", updatedGroup)
               .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateStudent(int studentId, Student updatedStudent)
        {
            await _client.Cypher
                .Match("(student: Student)")
                .Where("ID(student) = $studentId")
                .WithParam("studentId", studentId)
                .Set("student = $updatedStudent")
                .WithParam("updatedStudent", updatedStudent)
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
                        .Match("(g:Group), (s:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            ICypherFluentQuery<GroupDTO> ret = a.Return(() => new GroupDTO
            {
                Id = Return.As<int>("ID(g)"),
                Group = Return.As<Group>("g")
            });

            if (descending)
                ret = ret.OrderBy(orderBy);
            else
                ret = ret.OrderByDescending(orderBy);

            return await ret.Skip(from).Limit(to).ResultsAsync;
        }

        public async Task<int> GetGroupsCount(string filters, string userFilter)
        {
            var a = _client.Cypher
                        .Match("(g:Group), (s:Student)")
                        .Where(userFilter);

            if (!string.IsNullOrEmpty(filters))
                a = a.AndWhere(filters);

            var res = await a.Return<int>("count(g)").ResultsAsync;
            return res.Single();
        }
    }

}
