using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.PostService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GolosaTgBotApi.Controllers
{
    [Route("api/post/")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/Post/{postId}
        [HttpGet("{postId}")]
        public async Task<ActionResult<PostDto>> GetPostWithComments(long postId)
        {
            var post = _postService.GetPosts(postId, 3);



            return Ok(post);
        }
    }
}
