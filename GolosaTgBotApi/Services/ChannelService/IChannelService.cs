using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.ChannelService
{
    public interface IChannelService
    {
        Task<Channel> CreateNewChannel(long chatId);

        Task CheckOnChannelExisting(long id);

        Task AddLinkedChat(ChatFullInfo chatInfo);
    }
}