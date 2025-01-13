using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using Telegram.Bot.Types;


namespace GolosaTgBotApi.Services.ChannelService
{
    public class ChannelService: IChannelService
    {
        private readonly ITelegramService _telegram;
        private readonly IMariaService _mariaService;

        public ChannelService(ITelegramService telegramService, IMariaService mariaService)
        {
            _telegram = telegramService;
            _mariaService = mariaService;
        }
        public async Task CreateNewChannel(long chatId)
        {
            
            var newChannel = new Channel();
            var chatInfo = await _telegram.GetChatInfoById(chatId);   
            newChannel.Id = chatId;
            newChannel.OwnerId = await _telegram.GetChannelOwnerId(chatId);
            newChannel.Title = chatInfo.Title;
            //todo переделать, пока что тут айди файла храниться
            await _mariaService.CreateNewChannel(newChannel);
        }
        public async Task CheckOnChannelExisting(long id)
        {
            var channel = await _mariaService.GetChannelById(id);
            if (channel == null)
            {
                await CreateNewChannel(id);
            }
        }
    }
}
