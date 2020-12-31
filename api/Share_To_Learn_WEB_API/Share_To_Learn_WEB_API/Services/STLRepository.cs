using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.DTOs;

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

        public async Task<bool> StudentExists(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return<int>("count(student)")
                            .ResultsAsync;

            return res.Single() > 0;
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
                    .WithIdentifier(ownerId.ToString())
                    .Create("(owner)-[:ADMINISTRATED]->(group:Group {newGroup})")
                    .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId)
        {
            var res = await _client.Cypher
                            .Match("(st:Student),(gr:Group)")
                            //.Where((Student st) => st.Email == email)
                            //.AndWhere((Group gr) => gr.Name == groupName)
                            .Where("id(st)={" + studentId.ToString()+"}")
                            .AndWhere("id(gr)={"+groupId.ToString()+"}")
                            .Return(st => st.As<Student>())
                            .ResultsAsync;
            //TODO: change email and name to ID
            return res.Count() == 0;
        }

        public async Task AddStudentToGroup(int studentId, int groupId)
        {
            await _client.Cypher
                             .Match("(st:Student),(gr:Group)")
                             .Where("id(st)={studentId}")
                             .AndWhere("id(gr)={groupId}")
                             .Create("(st)-[:Member]->(gr)")
                             .ExecuteWithoutResultsAsync();
        }

        public async Task RemoveStudentFromGroup(int studentId, int groupId)
        {
            await _client.Cypher
                            .Match("(st:Student),(gr:Group)")
                            .Match("(st)-[r:Member]-(gr)")
                            .Where("id(st)={studentId}")
                            .AndWhere("id(gr)={groupId}")
                            .Delete("r")
                            .ExecuteWithoutResultsAsync();
        }
    }   
}
