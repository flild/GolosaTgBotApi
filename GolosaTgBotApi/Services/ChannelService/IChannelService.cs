namespace GolosaTgBotApi.Services.ChannelService
{
    public interface IChannelService
    {
        public Task CreateNewChannel(long chatId);
        public Task CheckOnChannelExisting(long id);
    }
}
