using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.ChannelService
{
    public class ChannelService : IChannelService
    {
        private readonly ITelegramService _telegram;
        private readonly IMariaService _mariaService;

        public ChannelService(ITelegramService telegramService, IMariaService mariaService)
        {
            _telegram = telegramService;
            _mariaService = mariaService;
        }

        public async Task CreateNewChannel(long channelId)
        {
            var channel = await _mariaService.GetChannelById(channelId);
            if (channel != null)
            {
                await UpdateChannelInfo(channel);
                return;
            }
            var newChannel = new Channel();
            var chatInfo = await _telegram.GetChatInfoById(channelId);
            newChannel.Id = channelId;
            newChannel.OwnerId = await _telegram.GetChannelOwnerId(channelId);
            newChannel.Title = chatInfo.Title;

            await _mariaService.CreateNewChannel(newChannel);
        }

        public async Task CheckOnChannelExisting(long channelId)
        {
            var channel = await _mariaService.GetChannelById(channelId);
            if (channel == null)
            {
                await CreateNewChannel(channelId);
            }
        }

        public async Task UpdateChannelInfo(Channel channel)
        {
            var chatInfo = await _telegram.GetChatInfoById(channel.Id);
            channel.OwnerId = await _telegram.GetChannelOwnerId(channel.Id);
            channel.Title = chatInfo.Title;
            await _mariaService.UpdateChannelInfo(channel);
        }
    }
}