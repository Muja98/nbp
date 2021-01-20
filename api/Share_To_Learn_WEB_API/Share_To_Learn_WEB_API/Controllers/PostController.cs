using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Share_To_Learn_WEB_API.Entities;
using Neo4jClient;
using Share_To_Learn_WEB_API.Services;
using Share_To_Learn_WEB_API.DTOs;
using Microsoft.AspNetCore.Http;

namespace Share_To_Learn_WEB_API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly ISTLRepository _repository;
        public PostController(ISTLRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("{groupId}/posts")]
       
        public async Task<ActionResult> GetAllPosts(int groupId)
        {
            IEnumerable<PostDTO> result = await _repository.GetAllPosts(groupId);
            foreach(PostDTO r in result)
            {
                r.Student.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(r.Student.Student.ProfilePicturePath);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("{groupId}/student/{studentId}")]
        public async Task<ActionResult> CreatePost(int groupId, int studentId, Post newPost)
        {
            PostDTO res = await _repository.CreatePost(groupId, studentId, newPost);
            res.Student.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(res.Student.Student.ProfilePicturePath);
            return Ok(res);
        }

        [HttpGet]
        [Route("{postId}/comments")]
        public async Task<ActionResult> GetAllComments(int postId)
        {
            IEnumerable<CommentDTO> result = await _repository.GetAllComment(postId);
            foreach(CommentDTO item in result)
            {
                item.Student.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(item.Student.Student.ProfilePicturePath);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("{postId}/student/{studentId}/newComment")]
        public async Task<IActionResult> CreateComment(int postId, int studentId, Comment newComment)
        {
            CommentDTO res = await _repository.CreateComment(postId, studentId, newComment);
            res.Student.Student.ProfilePicturePath = FileManagerService.LoadImageFromFile(res.Student.Student.ProfilePicturePath);
            return Ok(res);
        }

        [HttpDelete]
        [Route("comment/{commentId}/delete")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _repository.DeleteComment(commentId);
            return Ok();
        }

        [HttpPut]
        [Route("comment/{commentId}/edit")]
        public async Task<IActionResult> UpdateComment(int commentId, Comment comment)
        {
            await _repository.UpdateComment(commentId, comment);
            return Ok();
        }

        [HttpPut]
        [Route("{postId}/edit")]
        public async Task<IActionResult> UpdatePost(int postId, Post post)
        {
            await _repository.UpdatePost(postId, post);
            return Ok();
        }

        [HttpDelete]
        [Route("{postId}/delete")]

        public async Task<IActionResult> DeletePost(int postId)
        {
            await _repository.DeletePost(postId);
            return Ok();
        }
    }
}
