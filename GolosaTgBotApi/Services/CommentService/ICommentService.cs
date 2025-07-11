using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public interface ICommentService
    {
        public Task HandleComment(Message message);
        Task<IEnumerable<Comment>> GetCommentsByPostId(long postId, int limit, int offset);
    }
}
