using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public interface IMariaService
    {
        public Task SaveCommentAsync(Comment comment);
        public Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId);
        public Task<List<Comment>> GetCommentsByTreadId(int treadId, long chatId, int limit, int offset);
        public Task<Dictionary<(long ChatId, int ThreadId), int>> GetCommentCountByIds(Dictionary<long, HashSet<int>> chatsOfThreadIds);
        public Task<Models.User>? GetUserbyIdAsync(long userId);
        public Task CreateNewUser(Models.User user);
        public Task<Channel?> GetChannelById(long ChannelId);
        public Task<List<Channel>> GetChannelsByIds(List<long> ChannelIds);
        public Task CreateNewChannel(Channel channel);
        public Task<Post> GetPostInChatById(int? PostId, long ChatId);
        public Task CreateNewPost(Post post);
        public Task<Post> GetPostById(long id);
        public Task<List<Post>> GetLatestsPosts(int limit, int offset);
        public Task UpdatePostInChatId(Post post);
    }
}
