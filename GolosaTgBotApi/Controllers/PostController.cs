using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.PostService;
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

        // POST: api/post
        [HttpPost]
        public async Task<ActionResult<PostPreviewDto>> GetPost([FromBody] PostParams parameters)
        {
            var posts = await _postService.GetPosts(parameters.Limit, parameters.Offset);
            return Ok(posts);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<PostPreviewDto>> GetPost(int id)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
    }
    public class PostParams
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
