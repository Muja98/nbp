using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentDTO>> GetStudentsPage(string filter, string userFilter, string orderBy, bool descending, int from, int to, int user);
        Task<string> getNextId(bool isImage);
        Task<bool> CreateNonExistingStudent(StudentRegisterDTO newStudent);
        Task<bool> StudentExists(int studentId);
        Task<StudentDTO> StudentExists(string email);
        Task<string> GetPassword(string email);
        Task UpdateStudent(int studentId, Student updatedStudent);
        Task<int> GetStudentsCount(string filter, string userFilter);
        Task DeleteFriendRequest(int receiverId, string requestId, int senderId);
        Task AddFriend(int studentId1, int studentId2);
        Task RemoveFriend(int studentId1, int studentId2);
        Task<IEnumerable<StudentDTO>> GetFriendsPage(string filter, string userFilter, string orderBy, bool descending, int from, int to);
        Task<int> GetFriendsCount(string filter, int userId);
        Task<StudentDTO> GetSpecificStudent(int studentId, int requesterId);
        Task SendFriendRequest(int senderId, int receiverId, Request sender);
        Task<IEnumerable<RequestDTO>> GetFriendRequests(int receiverId);
        Task<IEnumerable<int>> GetFriendRequestSends(int senderId);
    }
}
