namespace GolosaTgBotApi.Services.TelegramService
{
    public interface ITelegramService
    {
        public Task<string> GetAvatarIdByUserId(long id);
        public Task<long?> GetChannelOwnerId(long chatId);
    }
}
