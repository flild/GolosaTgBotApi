using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.UserService
{
    public interface IUserService
    {
        public Task CreateNewUser(User user);
    }
}
