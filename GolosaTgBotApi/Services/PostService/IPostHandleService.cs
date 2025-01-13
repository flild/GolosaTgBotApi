using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.PostService
{
    public interface IPostHandleService
    {
        public Task HandlePost(Message post);
    }
}
