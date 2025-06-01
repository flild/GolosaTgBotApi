using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GolosaTgBotApi.Services.TelegramService
{
    //scoped service
    public class TelegramService : ITelegramService
    {
        private readonly ITelegramBotClient bot;

        public TelegramService()
        {
            Env.Load();
            bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"));
        }

        public async Task SendMessageInChat(long chatId, string message)
        {
            await bot.SendMessage(chatId, message);
        }

        public async Task<string> GetAvatarIdByUserId(long id)
        {
            var photo = await bot.GetUserProfilePhotos(id);
            return photo.Photos[0][0].FileId;
        }

        public async Task<long?> GetChannelOwnerId(long? chatId)
        {
            var admins = await bot.GetChatAdministrators(chatId);
            foreach (var admin in admins)
            {
                if (admin.Status == ChatMemberStatus.Creator)
                {
                    return admin.User.Id;
                }
            }
            return null;
        }

        public async Task<ChatFullInfo> GetChatInfoById(long chatId)
        {
            return await bot.GetChat(chatId);
        }

        public async Task<ChatMember[]> GetChatAdministrators(long chatId)
        {
            return await bot.GetChatAdministrators(chatId);
        }

        public async Task<bool> IsUserAdministrator(long chatId, long userId)
        {
            var chatMember = await bot.GetChatMember(chatId, userId);

            if (chatMember.Status == ChatMemberStatus.Creator || chatMember.Status == ChatMemberStatus.Administrator)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<MemoryStream> GetFileById(string fileId)
        {
            var stream = new MemoryStream();
            await bot.DownloadFile(fileId, stream);
            return stream;
        }

        public async Task TestAsync()
        {
            var userProfile = await bot.GetUserProfilePhotos(306903557);
            var fileId = userProfile.Photos[0][0].FileId;
            var fileInfo = await bot.GetFile("AgACAgIAAx0CTxROOQADt2eTidRaJ48VTDYN75IPZFnQQyq6AAJA6jEbTASZSLmhzaKlET7vAQADAgADbQADNgQ");
            var url = fileInfo.FilePath;
            Console.WriteLine(fileId);
            Console.WriteLine(url);
        }
    }
}