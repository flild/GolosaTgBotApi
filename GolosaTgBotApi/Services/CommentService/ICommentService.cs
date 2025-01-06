using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public interface ICommentService
    {
        public Task HandleComment(Message message);
    }
}
