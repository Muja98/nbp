using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;

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
    } 
}
