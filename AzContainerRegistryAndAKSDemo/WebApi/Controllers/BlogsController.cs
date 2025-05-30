﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IPostService postService, ILogger<BlogsController> logger) 
        {
            _postService = postService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postService.GetAllAsync();
            return Ok(posts);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await _postService.GetByIdAsync(id);
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }
            var response = await _postService.CreateAsync(post);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePost([FromBody] UpdatePostRequest post)
        {
            var response = await _postService.UpdateAsync(post);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            await _postService.DeleteAsync(id);
            return Ok();
        }
    }
}
