using DotNetEnv;
using Telegram.Bot;

namespace GolosaTgBotApi.Services.TelegramService
{
    public class TelegramService: ITelegramService
    {
        private readonly ITelegramBotClient bot;
        public TelegramService()
        {
            Env.Load();
            bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"));
        }
        public async Task<string> GetAvatarIdByUserId(long id)
        {
            var photo = await bot.GetUserProfilePhotos(id);
            return photo.Photos[0][0].FileId;
        }
        public async Task<long?> GetChannelOwnerId(long chatId)
        {
            var admins = await bot.GetChatAdministrators(chatId);
            foreach (var admin in admins)
            {
                if( admin.Status == Telegram.Bot.Types.Enums.ChatMemberStatus.Creator)
                {
                    return admin.User.Id;
                }
            }
            return null;
        }
    }
}
