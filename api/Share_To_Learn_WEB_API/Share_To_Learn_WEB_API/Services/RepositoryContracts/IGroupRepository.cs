using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.DTOs;

namespace Share_To_Learn_WEB_API.Services.RepositoryContracts
{
    public interface IGroupRepository
    {
        Task CreateGroup(int ownerId, Group newGroup);
        Task<bool> GroupExists(int groupId);
        Task UpdateGroup(int groupId, Group updatedGroup);
        Task<IEnumerable<GroupDTO>> GetGroupsPage(string filters, string userFilter, string orderBy, bool descending, int from, int to);
        Task<IEnumerable<GroupDTO>> GetStudentGroups(int studentId);
        Task<int> GetGroupsCount(string filters, string userFilter);
        Task DeleteGroup(int groupId);
        Task<bool> CheckIfStudentIsMemberOfAGroup(int studentId, int groupId);
        Task AddStudentToGroup(int studentId, int groupId);
        Task RemoveStudentFromGroup(int studentId, int groupId);
        Task<IEnumerable<GroupDTO>> GetMemberships(int studentId);
        Task<IEnumerable<GroupDTO>> GetOwnerships(int studentId);
        Task<IEnumerable<StudentDTO>> GetGroupMembers(int groupId, int requesterId);
        Task<string> GetStudentGroupRelationship(int studentId, int groupId);
        Task<string> GetGroupImage(int groupId);
        Task<GroupStatisticsDTO> GetGroupStatistics(int groupId);
        Task<StudentDTO> GetGroupOwner(int groupId, int requesterId);

    }
}
