using Telegram.Bot.Types;
using GolosaTgBotApi.Services.TelegramService;
using GolosaTgBotApi.Services.MariaService;

namespace GolosaTgBotApi.Services.UserService
{
    public class UserService: IUserService
    {
        private readonly ITelegramService _telegram;
        private readonly IMariaService _mariaService;

        public UserService(ITelegramService telegramService, IMariaService mariaService)
        {
            _telegram = telegramService;
            _mariaService = mariaService;
        }
        public async Task CreateNewUser(User user)
        {
            var newUser = new Models.User();
            newUser.Id = user.Id;
            newUser.Username = user.Username;
            var avatarId = await _telegram.GetAvatarIdByUserId(user.Id);
            //todo переделать, пока что тут айди файла храниться
            newUser.AvatarFileId = avatarId;
            await _mariaService.CreateNewUser(newUser);
        }
    }
}
