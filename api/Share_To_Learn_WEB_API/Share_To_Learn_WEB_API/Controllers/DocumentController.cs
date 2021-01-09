using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Share_To_Learn_WEB_API.Entities;
using Share_To_Learn_WEB_API.Services;
using Share_To_Learn_WEB_API.DTOs;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/documents/")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly ISTLRepository _repository;

        public DocumentController(ISTLRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Route("creator/{studentId}/group/{groupId}")]
        public async Task<ActionResult> CreateDocument(int studentId, int groupId, [FromBody] Document newDocument)
        {   
            newDocument.DocumentPath = FileManagerService.SaveDocumentToFile(newDocument.DocumentPath);
            await _repository.CreateDocument(studentId, newDocument);
            await _repository.RelateDocumentAndGroup(groupId, newDocument.DocumentPath);

            return Ok(newDocument);
        }

        [HttpGet]
        [Route("group/{groupId}")]
        public async Task<ActionResult> GetDocuments(int groupId)
        {
            var result = await _repository.GetDocuments(groupId);
            return Ok(result);
        }

        [HttpGet]
        [Route("from_path")]
        public async Task<ActionResult> GetDocument([FromBody] string path)
        {
            var result = FileManagerService.LoadDocumentFromFile(path);
            return Ok(result);
        }



    }
}
