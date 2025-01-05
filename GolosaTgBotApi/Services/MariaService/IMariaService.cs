using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public interface IMariaService
    {
        public void SaveCommentAsync(Message message);
        public Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId);

        public Task<Models.User>? GetUserbyIdAsync(long userId);

    }
}
