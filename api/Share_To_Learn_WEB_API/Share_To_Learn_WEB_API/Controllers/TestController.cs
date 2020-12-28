using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.Services;

namespace Share_To_Learn_WEB_API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ISTLRepository _repository;

        public TestController(ISTLRepository repository)
        {
            _repository = repository;
        }

        [HttpGet()]
        public async Task<ActionResult> GetStudents()
        {
            var result = await _repository.GetStudents();

            return Ok(result); 
        }

        [HttpPost]
        public async Task<ActionResult> CreateStudent([FromBody] Student newstudent)
        {
            //var newStudent = newstudent;
            //await _client.Cypher
            //    .Create("(student:Student $newStudent)")
            //    .WithParam("newStudent", newStudent)
            //    .ExecuteWithoutResultsAsync();

            //return Ok(newStudent);

            return Ok();
        }
    }
}

