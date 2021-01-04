using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using System.Security.Claims;

namespace Share_To_Learn_WEB_API.Services
{
    public class JwtManager
    {
        public static string GenerateJWToken(Student student, string id)
        {
            var claims = new[] {
                new Claim("id", id),
                new Claim("firstName", student.FirstName),
                new Claim("lastName", student.LastName),
                new Claim("email", student.Email),
                new Claim("dateOfBirth", student.DateOfBirth.ToShortDateString()),
                new Claim("profilePicturePath", student.ProfilePicturePath)
            };

            var token = new JwtSecurityToken(null, null, claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
