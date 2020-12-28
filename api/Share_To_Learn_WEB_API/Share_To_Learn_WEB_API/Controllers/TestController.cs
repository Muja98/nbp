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
        public async Task<ActionResult> CreateStudent([FromBody] StudentDTO newStudent)
        {
            //odvojiti u posebnu funkciju, najbolje u neki servis za autentifikaciju
            //----------------------------------------------------------
            byte[] salt = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(salt);
            byte[] pwdBytes = System.Text.Encoding.Unicode.GetBytes(newStudent.Password);
            byte[] combinedBytes = new byte[pwdBytes.Length + salt.Length];
            Buffer.BlockCopy(pwdBytes, 0, combinedBytes, 0, pwdBytes.Length);
            Buffer.BlockCopy(salt, 0, combinedBytes, pwdBytes.Length, salt.Length);
            System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
            byte[] hash = hashAlgo.ComputeHash(combinedBytes);
            byte[] hashPlusSalt = new byte[hash.Length + salt.Length];
            Buffer.BlockCopy(hash, 0, hashPlusSalt, 0, hash.Length);
            Buffer.BlockCopy(salt, 0, hashPlusSalt, hash.Length, salt.Length);
            newStudent.Password = System.Text.Encoding.UTF8.GetString(hashPlusSalt);
            //----------------------------------------------------------
            Student student = new Student
            {
                FirstName = newStudent.FirstName,
                LastName = newStudent.LastName,
                DateOfBirth = newStudent.DateOfBirth,
                Email = newStudent.Email,
                ProfilePicturePath = newStudent.ProfilePicturePath,
                Password = hashPlusSalt
            };


            if (await _repository.CreateNonExistingStudent(student))
                return Ok();
            else
                return BadRequest("Email taken");
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> LogUserIn([FromBody] AccountLogInDTO userCredentials)
        {
            byte[] savedPwd = await _repository.GetPassword(userCredentials.Email);
            if (savedPwd!=null)
            {
                byte[] saltBytes = savedPwd.Skip(savedPwd.Length - 32).ToArray();
                byte[] hashedPwdBytes = savedPwd.Take(savedPwd.Length - 32).ToArray();
                string hashedPwdString = System.Text.Encoding.UTF8.GetString(hashedPwdBytes);

                byte[] pwdBytes = System.Text.Encoding.Unicode.GetBytes(userCredentials.Password);
                byte[] combinedBytes = new byte[pwdBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(pwdBytes, 0, combinedBytes, 0, pwdBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, combinedBytes, pwdBytes.Length, saltBytes.Length);
                System.Security.Cryptography.HashAlgorithm hashAlgo = new System.Security.Cryptography.SHA256Managed();
                byte[] hash = hashAlgo.ComputeHash(combinedBytes);
                string stringHash = System.Text.Encoding.UTF8.GetString(hash);

                if (hashedPwdString == stringHash)
                    return Ok();
                else
                    return BadRequest("Wrong password");
            }
            else
                return BadRequest("Non-existent email");
        }
    }
}

