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
        /// <summary>
        /// Получить комментарии к посту с пагинацией
        /// </summary>
        /// <param name="postId">Идентификатор поста</param>
        /// <param name="limit">Количество комментариев на странице</param>
        /// <param name="offset">Смещение</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost(
            long postId,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0)
        {
            var comments = await _commentService.GetCommentsByPostId(postId, limit, offset);
            return Ok(comments);
        }

        /// <summary>
        /// Получить ответы (ветку) к комментарию с пагинацией
        /// </summary>
        /// <param name="postId">Идентификатор поста (для контекста маршрута)</param>
        /// <param name="commentId">Идентификатор родительского комментария</param>
        /// <param name="limit">Количество ответов на странице</param>
        /// <param name="offset">Смещение</param>
        [HttpGet("{commentId}/replies")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetRepliesForComment(
            long postId,
            long commentId,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0)
        {
            var replies = await _commentService.GetRepliesByCommentId(commentId, limit, offset);
            return Ok(replies);
        }
        public class CommentsParams
        {
            public long PostId {  get; set; }
            public int Limit { get; set; }
            public int Offset { get; set; }
        }
    }
}
