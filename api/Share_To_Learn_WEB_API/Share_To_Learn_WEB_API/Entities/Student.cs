using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Share_To_Learn_WEB_API.Entities
{
    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
