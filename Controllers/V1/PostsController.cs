using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Contracts.V1;
using TweetAPI.Controllers.V1.Requests;
using TweetAPI.Controllers.V1.Responses;
using TweetAPI.Domain;
using TweetAPI.Extensions;
using TweetAPI.Services;

namespace TweetAPI.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        private readonly IPostService PostService;

        public PostsController(IPostService postService)
        {
            PostService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {

            var posts = await PostService.GetAllAsync();

            return Ok(posts);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> GetPost([FromRoute] Guid postId)
        {
            var post = await PostService.GetPostAsync(postId);

            if (post != null)
            {
                var response = new PostResponse() { Id = post.Id, Name = post.Name };
                return Ok(response);
            }
            return NotFound();
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] PostRequestToUpdate postToUpdate)
        {
            var currentUser = HttpContext.GetUserId();

            var exists = await PostService.GetPostAsync(postId);

            if (!exists.UserId.Equals(currentUser))
            {
                return BadRequest(new { error = "You are not authroized to change this post" });
            }

            var post = new Post { Id = postId, Name = postToUpdate.Name };

            if (exists != null)
            {
                var postService = await PostService.UpdatePostAsync(postId, post);
                if (postService)
                    return Ok(postToUpdate);
            }
            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var post = await PostService.DeletePostAsync(postId);

            if (post)
                return NoContent();

            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create([FromBody] PostRequest postToCreate)
        {
            var post = new Post { Name = postToCreate.Name, UserId = HttpContext.GetUserId() };
            await PostService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}/{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());
            
            var resposne = new PostResponse() { Id = post.Id, Name = post.Name };

            return Created(locationUri, resposne);

        }
    }
}
