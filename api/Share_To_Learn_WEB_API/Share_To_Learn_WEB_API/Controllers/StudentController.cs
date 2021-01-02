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
using Share_To_Learn_WEB_API.DTOs;
using Microsoft.AspNetCore.Http;

namespace Share_To_Learn_WEB_API.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly ISTLRepository _repository;

        public StudentController(ISTLRepository repository)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateStudent([FromBody] StudentRegisterDTO newStudent)
        {
            newStudent.Password = AuthentificationService.EncryptPassword(newStudent.Password);
            if (await _repository.CreateNonExistingStudent(newStudent))
            {
                StudentDTO student = await _repository.StudentExists(newStudent.Student.Email);
                string token = JwtManager.GenerateJWToken(student.Student, student.Id.ToString());
                return Ok(new JsonResult(token));
            }
            else
                return BadRequest(new JsonResult("Email taken"));
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LogUserIn([FromBody] AccountLogInDTO userCredentials)
        {
            string savedPwd = await _repository.GetPassword(userCredentials.Email);
            if (savedPwd!=null)
            {
                if(AuthentificationService.IsPasswordCorrect(savedPwd, userCredentials.Password))
                {
                    StudentDTO student = await _repository.StudentExists(userCredentials.Email);
                    string token = JwtManager.GenerateJWToken(student.Student, student.Id.ToString());
                    return Ok(new JsonResult(token));
                }
                else
                    return BadRequest("Wrong password");
            }
            else
                return BadRequest("Non-existent email");
        }

        [HttpPut("{studentId}")]
        public async Task<ActionResult> UpdateStudent(int studentId, Student updatedStudent)
        {
            bool res = await _repository.StudentExists(studentId);

            if (!res)
                return BadRequest("Student doesnt exist!");

            await _repository.UpdateStudent(studentId, updatedStudent);
            return Ok(updatedStudent);
        }


    }
}

