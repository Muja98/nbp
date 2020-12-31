using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult> CreateGroup(int ownerId, Group newGroup)
        {
            await _repository.CreateGroup(ownerId, newGroup);
            return Ok(newGroup);
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
    }
}
