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
            var res =   await _client.Cypher
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

        public async Task<byte[]> GetPassword(string email)
        {
            var res = await _client.Cypher
                            .Match("(student:Student)")
                            .Where((Student student) => student.Email == email)
                            .Return<byte[]>("student.Password")
                            .ResultsAsync;

            if (res.Count() > 0)
                return res.Single();
            else
                return null;
        }
    } 
}
