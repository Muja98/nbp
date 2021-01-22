using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.RedisConnection;
using Share_To_Learn_WEB_API.Services;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;
using StackExchange.Redis;
using Share_To_Learn_WEB_API.Services.RepositoryContracts;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDocumentRepository _documentrepository;
        private readonly IGroupRepository _repository;
        private readonly ISharedRepository _sharedRepository;

        public GroupController(IGroupRepository repository, ISharedRepository sharedRepository, IRedisConnectionBuilder builder, IDocumentRepository documentrepository)
        {
            _documentrepository = documentrepository;
            _repository = repository;
            _sharedRepository = sharedRepository;
            _redisConnection = builder.Connection;
        }

        [HttpPost]
        [Route("{ownerId}")]
        public async Task<ActionResult> CreateGroup(int ownerId, Group newGroup)
        {
            string imageFileId = await _sharedRepository.GetNextImageId();
            newGroup.GroupPicturePath = FileManagerService.SaveImageToFile(newGroup.GroupPicturePath, imageFileId);
            await _repository.CreateGroup(ownerId, newGroup);
            return Ok();
        }

        [HttpPut]
        [Route("{groupId}")]
        public async Task<ActionResult> UpdateGroup(int groupId, Group newGroup)
        {
            bool res = await _repository.GroupExists(groupId);

            if (!res)
                return BadRequest("Group doesnt exist!");

            string imageFileId = await _sharedRepository.GetNextImageId();
            newGroup.GroupPicturePath = FileManagerService.SaveImageToFile(newGroup.GroupPicturePath, imageFileId);
            await _repository.UpdateGroup(groupId, newGroup);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetFilteredGroups([FromQuery] string name, [FromQuery] string field, [FromQuery] bool orderByName, [FromQuery] bool descending, [FromQuery] int from, [FromQuery] int to, [FromQuery] int user)
        {
            string userFilter = "not (s)-[:MEMBER|:OWNER]->(g) and ID(s) = " + user;
            string where1 = (string.IsNullOrEmpty(name) ? "" : ("g.Name =~\"(?i).*" + name + ".*\"")); 
            string where2 = (string.IsNullOrEmpty(field) ? "" : ("g.Field =~ \"(?i).*" + field + ".*\""));
            string where = "";
            if (!string.IsNullOrEmpty(where1) && !string.IsNullOrEmpty(where2))
                where += where1 + " AND " + where2;
            else if (!string.IsNullOrEmpty(where1))
                where += where1;
            else if (!string.IsNullOrEmpty(where2))
                where += where2;

            string order = orderByName ? "g.Name" : "ID(g)";

            IEnumerable<GroupDTO> groups;
            groups = await _repository.GetGroupsPage(where, userFilter, order, descending, from, to);

            foreach(GroupDTO g in groups)
            {
                g.Group.GroupPicturePath = FileManagerService.LoadImageFromFile(g.Group.GroupPicturePath);
            }
            return Ok(new JsonResult(groups));
        }

        [HttpGet]
        [Route("group-count")]
        public async Task<ActionResult> GetFilteredGroupsCount([FromQuery] string name, [FromQuery] string field, [FromQuery] int user)
        {
            string userFilter = "not (s)-[:MEMBER|:OWNER]->(g) and ID(s) = " + user;
            string where1 = (string.IsNullOrEmpty(name) ? "" : ("g.Name = \"(?i).*" + name + ".*\""));
            string where2 = (string.IsNullOrEmpty(field) ? "" : ("g.Field = \"(?i).*" + field + ".*\""));
            string where = "";
            if (!string.IsNullOrEmpty(where1) && !string.IsNullOrEmpty(where2))
                where += where1 + " AND " + where2;
            else if (!string.IsNullOrEmpty(where1))
                where += where1;
            else if (!string.IsNullOrEmpty(where2))
                where += where2;

            int groupsNum;
            groupsNum = await _repository.GetGroupsCount(where, userFilter);

            return Ok(new JsonResult(groupsNum));
        }

        [HttpPost]
        [Route("student/{studentId}/group/{groupId}")]
        public async Task<ActionResult> JoinGroup(int studentId, int groupId)
        {
            bool check = await _repository.CheckIfStudentIsMemberOfAGroup(studentId, groupId);
            if (check)
            {
                await _repository.AddStudentToGroup(studentId, groupId);
                return Ok();
            }
            else return BadRequest("Student is already member of this group");
        }

        [HttpDelete]
        [Route("delete/student/{studentId}/group/{groupId}")]
        public async Task<IActionResult> UnJoinGroup(int studentId, int groupId)
        {
            await _repository.RemoveStudentFromGroup(studentId, groupId);
            return Ok();
        }

        [HttpGet]
        [Route("student/{studentId}/memberships")]
        public async Task<ActionResult> GetMyMemberships(int studentId)
        {
            var result = await _repository.GetMemberships(studentId);

            foreach (GroupDTO g in result)
            {
                g.Group.GroupPicturePath = FileManagerService.LoadImageFromFile(g.Group.GroupPicturePath);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("student/{studentId}/ownerships")]
        public async Task<ActionResult> GetMyOwnerships(int studentId)
        {
            var result = await _repository.GetOwnerships(studentId);

            foreach (GroupDTO g in result)
            {
                g.Group.GroupPicturePath = FileManagerService.LoadImageFromFile(g.Group.GroupPicturePath);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("student/{studentId}/groups")]
        public async Task<ActionResult> GetStudentGroups(int studentId)
        {
            IEnumerable<GroupDTO> result = await _repository.GetStudentGroups(studentId);
            foreach( GroupDTO res in result)
            {
                res.Group.GroupPicturePath = FileManagerService.LoadImageFromFile(res.Group.GroupPicturePath);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("{groupId}/members/student/{studentId}")]
        public async Task<ActionResult> GetGroupMembers(int groupId, int studentId)
        {
            var result = await _repository.GetGroupMembers(groupId, studentId);

            foreach(StudentDTO s in result)
                s.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(s.Student.ProfilePicturePath);

            return Ok(result);
        }

        [HttpGet]
        [Route("{groupId}/owner/student/{studentId}")]
        public async Task<ActionResult> GetGroupOwner(int groupId, int studentId)
        {
            var result = await _repository.GetGroupOwner(groupId, studentId);
            result.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(result.Student.ProfilePicturePath);
            return Ok(result);
        }

        [HttpGet]
        [Route("{groupId}/statistics")]
        public async Task<ActionResult> GetGroupStatistics(int groupId)
        {
            GroupStatisticsDTO result = await _repository.GetGroupStatistics(groupId);
            result.Group.GroupPicturePath = FileManagerService.LoadImageFromFile(result.Group.GroupPicturePath);
            return Ok(result);
        }

        [HttpGet]
        [Route("relationship/student/{studentId}/group/{groupId}")]
        public async Task<ActionResult> GetUserGroupRelationship(int studentId, int groupId)
        {
            var result = await _repository.GetStudentGroupRelationship(studentId, groupId);
            return Ok(new { type = result});
        }

       [HttpGet]
       [Route ("{groupId}/groupImage")]
        public async Task<ActionResult> getGroupImage(int groupId)
        {
            string path = await _repository.GetGroupImage(groupId);
            path = FileManagerService.LoadImageFromFile(path);
            return Ok(new { image = path });
        }

        [HttpDelete]
        [Route ("{groupId}")]

        public async Task<ActionResult> DeleteGroup(int groupId)
        {
            string patha = await _repository.GetGroupImage(groupId);
            if(!string.IsNullOrEmpty(patha))
                FileManagerService.DeleteFile(patha);

            IEnumerable<string> paths = await _documentrepository.GetDocumentsPaths(groupId);
            
            if(paths!=null)
            { 
                foreach (string path in paths) 
                { 
                    if(!string.IsNullOrEmpty(path))
                        FileManagerService.DeleteFile(path);
                }
            }

            await _repository.DeleteGroup(groupId);

            return Ok();
        }
    }
}
