using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public interface IMariaService
    {
        public void SaveCommentAsync(Message message);
    }
}
