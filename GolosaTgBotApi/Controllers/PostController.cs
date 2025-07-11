using GolosaTgBotApi.Models;
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

        /// <summary>
        /// Получить ленту постов с пагинацией
        /// </summary>
        /// <param name="limit">Количество постов на странице</param>
        /// <param name="offset">Смещение</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostPreviewDto>>> GetFeed(
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0)
        {
            var posts = await _postService.GetPosts(limit, offset);
            return Ok(posts);
        }

        /// <summary>
        /// Получить конкретный пост по ID
        /// </summary>
        /// <param name="id">Идентификатор поста</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPostById(long id)
        {
            var post = await _postService.GetPostById(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }
    }
    public class PostParams
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
