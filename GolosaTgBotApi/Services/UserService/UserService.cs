using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.UserService
{
    public class UserService: IUserService
    {
        public async Task CreateNewUser(User user)
        {
            var newUser = new Models.User();
            newUser.Id = user.Id;
            newUser.Username = user.Username;
        }
    }
}
