﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;

namespace Share_To_Learn_WEB_API.Services
{
    public interface ISTLRepository
    {
        Task<IEnumerable<Student>> GetStudents();
        Task<bool> StudentExists(string email);
        Task CreateStudent(Student newStudent);
        Task<bool> CreateNonExistingStudent(Student newStudent);
        Task<string> GetPassword(string email);
        Task CreateGroup(int ownerId, Group newGroup);
        Task UpdateGroup(int groupId, Group updatedGroup);
        Task UpdateStudent(int studentId, Student updatedStudent);
        Task<bool> StudentExists(int studentId);
        Task<bool> GroupExists(int groupId);
    }
}
