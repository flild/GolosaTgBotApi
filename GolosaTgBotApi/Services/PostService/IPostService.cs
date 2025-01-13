using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using Telegram.Bot.Types;
namespace GolosaTgBotApi.Services.PostService
{
    public interface IPostService
    {
        public Task<List<PostPreviewDto>> GetPosts(int limit, int offset);
        public Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId);
        public Task<Post> GetPostById(long id);

    }
}
