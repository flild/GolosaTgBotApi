using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public interface IMariaService
    {
        public Task SaveCommentAsync(Comment comment);
        public Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId);

        public Task<Models.User>? GetUserbyIdAsync(long userId);
        public Task CreateNewUser(Models.User user);
        public Task<Channel?> GetChannelById(long ChannelId);
        public Task CreateNewChannel(Channel channel);
        public Task<Post> GetPostInChatById(int? PostId, long ChatId);
        public Task CreateNewPost(Post post);
        public Task UpdatePostInChatId(Post post);
    }
}
