using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Share_To_Learn_WEB_API.DTOs;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.Services;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly ISTLRepository _repository;

        public GroupController(ISTLRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("{ownerId}")]
        public async Task<ActionResult> CreateGroup(int ownerId, Group newGroup)
        {
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

            await _repository.UpdateGroup(groupId, newGroup);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetFilteredGroups([FromQuery] string name, [FromQuery] string field, [FromQuery] bool orderByName, [FromQuery] bool descending, [FromQuery] int from, [FromQuery] int to)
        {
            string where1 = (string.IsNullOrEmpty(name) ? "" : ("g.Name = \"" + name + "\"")); 
            string where2 = (string.IsNullOrEmpty(field) ? "" : ("g.Field = \"" + field + "\""));
            string where = "";
            if (!string.IsNullOrEmpty(where1) && !string.IsNullOrEmpty(where2))
                where += where1 + " AND " + where2;
            else if (!string.IsNullOrEmpty(where1))
                where += where1;
            else if (!string.IsNullOrEmpty(where2))
                where += where2;

            string order = orderByName ? "g.Name" : "ID(g)";

            IEnumerable<GroupDTO> groups;
            if (descending)
                groups = await _repository.GetGroupsPageDesc(where, order, from, to);
            else
                groups = await _repository.GetGroupsPage(where, order, from, to);

            return Ok(new JsonResult(groups));
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
            if (!await _repository.StudentExists(studentId))
                return Ok("Student doesnt exists");

            var result = await _repository.GetMemberships(studentId);
            return Ok(result);
        }

        [HttpGet]
        [Route("student/{studentId}/ownerships")]
        public async Task<ActionResult> GetMyOwnerships(int studentId)
        {
            var result = await _repository.GetOwnerships(studentId);
            return Ok(result);
        }
    }
}
