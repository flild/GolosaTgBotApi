using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public interface ICommentService
    {
        public Task HandleComment(Message message);
        public Task<List<Comment>> GetCommentsByPostId(long PostId, int limit, int offset);
    }
}
