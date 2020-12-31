using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;

namespace Share_To_Learn_WEB_API.Services
{
    public interface ISTLRepository
    {
        Task<IEnumerable<Student>> GetStudents();
        Task<StudentDTO> StudentExists(string email);
        Task CreateStudent(Student newStudent);
        Task<bool> CreateNonExistingStudent(Student newStudent);
        Task<string> GetPassword(string email);
        Task CreateGroup(int ownerId, Group newGroup);
<<<<<<< HEAD
        Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId);
        Task AddStudentToGroup(int studentId, int groupId);
        Task RemoveStudentFromGroup(int studentId, int groupId);
=======
        Task UpdateGroup(int groupId, Group updatedGroup);
        Task UpdateStudent(int studentId, Student updatedStudent);
        Task<bool> StudentExists(int studentId);
        Task<bool> GroupExists(int groupId);
        Task<IEnumerable<GroupDTO>> GetGroupsPage(string filters, string orderBy, int from, int to);
        Task<IEnumerable<GroupDTO>> GetGroupsPageDesc(string filters, string orderBy, int from, int to);
>>>>>>> b373b6f3559430752762214ef943ddc7fa393ef8
    }
}
