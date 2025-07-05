using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public interface ICommentService
    {
        public Task HandleComment(Message message);
        Task<IEnumerable<CommentDto>> GetCommentsByPostId(long postId, int limit, int offset);
        Task<IEnumerable<CommentDto>> GetRepliesByCommentId(long commentId, int limit, int offset);
    }
}
