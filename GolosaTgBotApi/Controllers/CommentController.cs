using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.CommentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GolosaTgBotApi.Controllers
{
    [Route("api/comment/")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        // POST: api/comment/post
        [HttpPost]
        [Route("post")]
        public async Task<ActionResult<PostPreviewDto>> GetCommentsByPostId([FromBody] CommentsParams parameters)
        {
            var posts = await _commentService.GetCommentsByPostId(parameters.PostId,parameters.Limit, parameters.Offset);
            return Ok(posts);
        }
        public class CommentsParams
        {
            public long PostId {  get; set; }
            public int Limit { get; set; }
            public int Offset { get; set; }
        }
    }
}
