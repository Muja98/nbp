using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Neo4jClient;
using Neo4jClient.Cypher;
using Share_To_Learn_WEB_API.Entities;


namespace Share_To_Learn_WEB_API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private GraphClient client = 
            new GraphClient(new Uri("http://localhost:7474/"), "neo4j", "sharetolearn");

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("MATCH (n:Student) RETURN n", new Dictionary<string, object>(), CypherResultMode.Set);

            List<Student> students = ((IRawGraphClient)client).ExecuteGetCypherResults<Student>(query).ToList();

            return Ok();
        }
    }
}
  