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
        Task<bool> CreateNonExistingStudent(StudentRegisterDTO newStudent);
        Task<string> GetPassword(string email);
        Task CreateGroup(int ownerId, Group newGroup);
        Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId);
        Task AddStudentToGroup(int studentId, int groupId);
        Task RemoveStudentFromGroup(int studentId, int groupId);
        Task UpdateGroup(int groupId, Group updatedGroup);
        Task UpdateStudent(int studentId, Student updatedStudent);
        Task<bool> StudentExists(int studentId);
        Task<bool> GroupExists(int groupId);
        Task<IEnumerable<GroupDTO>> GetMemberships(int studentId);
        Task<IEnumerable<GroupDTO>> GetOwnerships(int studentId);
        Task<IEnumerable<GroupDTO>> GetGroupsPage(string filters, string userFilter, string orderBy, bool descending, int from, int to);
        Task<int> GetGroupsCount(string filters, string userFilter);
        Task<IEnumerable<StudentDTO>> GetGroupMembers(int groupId);
        Task<StudentDTO> GetGroupOwner(int groupId);
    }
}
