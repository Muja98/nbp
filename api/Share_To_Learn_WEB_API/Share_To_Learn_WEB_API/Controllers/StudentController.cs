using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.Services;
using Share_To_Learn_WEB_API.DTOs;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using Share_To_Learn_WEB_API.RedisConnection;

namespace Share_To_Learn_WEB_API.Controllers
{
    [ApiController]
    [Route("api/student")]
    public class StudentController : ControllerBase
    {
        private readonly ISTLRepository _repository;
        private readonly IConnectionMultiplexer _redisConnection;

        public StudentController(ISTLRepository repository, IRedisConnectionBuilder builder)
        {
            _repository = repository;
            _redisConnection = builder.Connection;
        }

        [HttpGet()]
        public async Task<ActionResult> GetFilteredStudents([FromQuery]string firstName, [FromQuery]string lastName, [FromQuery]bool orderByName, [FromQuery]bool descending , int from, int to, int user)
        {
            string userFilter = "not ID(s)=" + user;
            string whereFirstName = string.IsNullOrEmpty(firstName) ? "" : ("s.FirstName=~\"(?i).*" + firstName + ".*\"");
            string whereLastName = string.IsNullOrEmpty(lastName) ? "" : ("s.LastName=~\"(?i).*" + lastName + ".*\"");
            string where = "";
            if (!string.IsNullOrEmpty(whereFirstName) && !string.IsNullOrEmpty(whereLastName))
                where += whereFirstName + " AND " + whereLastName;
            else if (!string.IsNullOrEmpty(whereFirstName))
                where += whereFirstName;
            else if (!string.IsNullOrEmpty(whereLastName))
                where += whereLastName;

            string order = "";
            if (!orderByName)
            { 
                order += "ID(s)";
                if (descending)
                    order += " desc";
            }
            else
            {
                if (descending)
                    order += "s.FirstName desc, s.LastName desc";
                else
                    order += "s.FirstName, s.LastName";
            }
            IEnumerable<StudentDTO> students;

            students = await _repository.GetStudentsPage(where, userFilter, order, descending, from, to);
            foreach (StudentDTO s in students)
                s.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(s.Student.ProfilePicturePath);

            return Ok(students);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CreateStudent([FromBody] StudentRegisterDTO newStudent)
        {
            newStudent.Password = AuthentificationService.EncryptPassword(newStudent.Password);
            string base64Image = newStudent.Student.ProfilePicturePath;
            if(!string.IsNullOrEmpty(base64Image))
                newStudent.Student.ProfilePicturePath = FileManagerService.SaveImageToFile(base64Image);
            if (await _repository.CreateNonExistingStudent(newStudent))
            {
                StudentDTO student = await _repository.StudentExists(newStudent.Student.Email);
                student.Student.ProfilePicturePath = base64Image;
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
                    student.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(student.Student.ProfilePicturePath);
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
        public async Task<ActionResult> UpdateStudent(int studentId, [FromBody] Student updatedStudent)
        {
            bool res = await _repository.StudentExists(studentId);
   
            if (!res)
                return BadRequest("Student doesnt exist!");
            updatedStudent.ProfilePicturePath = FileManagerService.SaveImageToFile(updatedStudent.ProfilePicturePath);
            await _repository.UpdateStudent(studentId, updatedStudent);
            return Ok(updatedStudent);
        }

        [HttpGet]
        [Route("student-count")]
        public async Task<ActionResult> GetFilteredStudentsCount([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] int user)
        {
            string userFilter = "not ID(s)=" + user;
            string whereFirstName = string.IsNullOrEmpty(firstName) ? "" : ("s.FirstName=~\"(?i).*" + firstName + ".*\"");
            string whereLastName = string.IsNullOrEmpty(lastName) ? "" : ("s.LastName=~\"(?i).*" + lastName + ".*\"");
            string where = "";
            if (!string.IsNullOrEmpty(whereFirstName) && !string.IsNullOrEmpty(whereLastName))
                where += whereFirstName + " AND " + whereLastName;
            else if (!string.IsNullOrEmpty(whereFirstName))
                where += whereFirstName;
            else if (!string.IsNullOrEmpty(whereLastName))
                where += whereLastName;

            int studentsCnt;
            studentsCnt = await _repository.GetStudentsCount(where, userFilter);

            return Ok(studentsCnt);
        }

        [HttpPost]
        [Route("friendship/sender/{senderId}/receiver/{receiverId}/request/{requestId}")]
        public async Task<ActionResult> AcceptFriendRequest(int senderId, int receiverId, string requestId)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            IDatabase redisDB = _redisConnection.GetDatabase();
            long deletedMessages = await redisDB.StreamDeleteAsync(channelName, new RedisValue[] { new RedisValue(requestId) });

            if(deletedMessages != 1)
                return BadRequest("There is problem on server");

            bool res1 = await _repository.StudentExists(senderId);
            bool res2 = await _repository.StudentExists(receiverId);

            if(res1&&res2)
            {
                await _repository.AddFriend(senderId, receiverId);
                return Ok();
            }
            else
                return BadRequest("Student doesnt exist!");

        }

        [HttpDelete]
        [Route("{studentId1}/student/{studentId2}")]
        public async Task<ActionResult> RemoveFriend(int studentId1, int studentId2)
        {
            bool res1 = await _repository.StudentExists(studentId1);
            bool res2 = await _repository.StudentExists(studentId2);

            if (res1 && res2)
            {
                await _repository.RemoveFriend(studentId1, studentId2);
                return Ok();
            }
            else
                return BadRequest("Student doesnt exist!");
        }

        [HttpGet]
        [Route("friends")]
        public async Task<ActionResult> GetFilteredFriends([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] bool orderByName, [FromQuery] bool descending, int from, int to, int user)
        {
            string userFilter = "ID(s)=" + user;
            string whereFirstName = string.IsNullOrEmpty(firstName) ? "" : ("s.FirstName=~\"(?i).*" + firstName + ".*\"");
            string whereLastName = string.IsNullOrEmpty(lastName) ? "" : ("s.LastName=~\"(?i).*" + lastName + ".*\"");
            string where = "";
            if (!string.IsNullOrEmpty(whereFirstName) && !string.IsNullOrEmpty(whereLastName))
                where += whereFirstName + " AND " + whereLastName;
            else if (!string.IsNullOrEmpty(whereFirstName))
                where += whereFirstName;
            else if (!string.IsNullOrEmpty(whereLastName))
                where += whereLastName;

            string order = "";
            if (!orderByName)
            {
                order += "ID(s)";
                if (descending)
                    order += " desc";
            }
            else
            {
                if (descending)
                    order += "s.FirstName desc, s.LastName desc";
                else
                    order += "s.FirstName, s.LastName";
            }
            IEnumerable<StudentDTO> students;

            students = await _repository.GetFriendsPage(where, userFilter, order, descending, from, to);

            foreach (StudentDTO s in students)
                s.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(s.Student.ProfilePicturePath);

            return Ok(students);
        }

        [HttpGet]
        [Route("friend-count")]
        public async Task<ActionResult> GetFilteredFriendsCount([FromQuery] string firstName, [FromQuery] string lastName, [FromQuery] int user)
        {
            string whereFirstName = string.IsNullOrEmpty(firstName) ? "" : ("s.FirstName=~\"(?i).*" + firstName + ".*\"");
            string whereLastName = string.IsNullOrEmpty(lastName) ? "" : ("s.LastName=~\"(?i).*" + lastName + ".*\"");
            string where = "";
            if (!string.IsNullOrEmpty(whereFirstName) && !string.IsNullOrEmpty(whereLastName))
                where += whereFirstName + " AND " + whereLastName;
            else if (!string.IsNullOrEmpty(whereFirstName))
                where += whereFirstName;
            else if (!string.IsNullOrEmpty(whereLastName))
                where += whereLastName;

            int studentsCnt;
            studentsCnt = await _repository.GetFriendsCount(where, user);

            return Ok(studentsCnt);
        }

        [HttpGet]
        [Route("student/{studentId}")]
        public async Task<ActionResult> GetSpecificStudent(int studentId)
        {
            StudentDTO student = await _repository.GetSpecificStudent(studentId);
            return Ok(student);
        }

        [HttpPost]
        [Route("friend_request/sender/{senderId}/receiver/{receiverId}")]
        public async Task<ActionResult> SendFriendRequest(int senderId, int receiverId, [FromBody] Student sender)
        {
            string channelName = $"messages:{receiverId}:friend_request";

            var values = new NameValueEntry[]
            {
                new NameValueEntry("sender_id", senderId),
                new NameValueEntry("sender_first_name", sender.FirstName),
                new NameValueEntry("sender_last_name", sender.LastName),
                new NameValueEntry("sender_email", sender.Email)
            };

            IDatabase redisDB = _redisConnection.GetDatabase();
            var messageId =  await redisDB.StreamAddAsync(channelName, values);

            return Ok("request successfuly sent");
        }

        //[HttpDelete]
        //[Route("friend_request/{requestId}")]
        //public async Task<ActionResult> DeleteFriendRequest(int requestId)
        //{

        //    return Ok();
        //}

    }
}
