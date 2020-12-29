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
    }
}
