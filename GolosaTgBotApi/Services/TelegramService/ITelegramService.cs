using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.TelegramService
{
    public interface ITelegramService
    {
        public Task SendMessageInChat(long chatId, string message);
        public Task<string> GetAvatarIdByUserId(long id);
        public Task<long?> GetChannelOwnerId(long? chatId);
        public Task<ChatFullInfo> GetChatInfoById(long chatId);
        public Task<ChatMember[]> GetChatAdministrators(long chatId);
        public Task<bool> IsUserAdministrator(long chatId, long userId);

    }
}
