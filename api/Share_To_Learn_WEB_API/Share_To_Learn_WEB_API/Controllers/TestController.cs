using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;

namespace Share_To_Learn_WEB_API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private readonly IGraphClient _client;

        public TestController(ILogger<TestController> logger, IGraphClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> GetStudents()
        {            
            var result = await _client.Cypher
                        .Match(@"(student:Student)")
                        .Return(student => new
                        {
                            Student = student.As<Student>()
                        }).ResultsAsync;

            _logger.LogInformation("Operation successful");

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> CreateStudent([FromBody] Student newstudent)
        {
            var newStudent = newstudent;
            await _client.Cypher
                .Create("(student:Student $newStudent)")
                .WithParam("newStudent", newStudent)
                .ExecuteWithoutResultsAsync();

            return Ok(newStudent);
        }
    }
}

