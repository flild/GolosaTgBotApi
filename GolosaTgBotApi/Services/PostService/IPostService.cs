using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using Telegram.Bot.Types;
namespace GolosaTgBotApi.Services.PostService
{
    public interface IPostService
    {
        public Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId);
        Task<IEnumerable<PostPreviewDto>> GetPosts(int limit, int offset);
        Task<Post> GetPostById(long id);

    }
}
