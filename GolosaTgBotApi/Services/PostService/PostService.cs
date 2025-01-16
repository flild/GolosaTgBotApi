using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.MariaService;

namespace GolosaTgBotApi.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMariaService _mariaService;

        public PostService(IMariaService mariaService)
        {
            _mariaService = mariaService;
        }

        public async Task<List<PostPreviewDto>> GetPosts(int limit, int offset)
        {
            var posts = await _mariaService.GetLatestsPosts(limit, offset);
            var channelIds = posts.Select(p => p.ChannelId).Distinct().ToList();
            var channelsDict = (await _mariaService.GetChannelsByIds(channelIds))
                .ToDictionary(ch => ch.Id);

            // Уникальная структура для хранения (ChatId, ThreadId)
            var chatsOfThreadIds = new Dictionary<long, HashSet<int>>();
            foreach (var post in posts)
            {
                if (!channelsDict.TryGetValue(post.ChannelId, out var channel) || channel.LinkedChatId == null)
                    continue;

                var chatId = channel.LinkedChatId.Value;
                if (!chatsOfThreadIds.ContainsKey(chatId))
                    chatsOfThreadIds[chatId] = new HashSet<int>();

                chatsOfThreadIds[chatId].Add(post.InChatId);
            }

            // Получаем словарь с подсчётом комментариев
            var commentsCountDict = await _mariaService.GetCommentCountByIds(chatsOfThreadIds);

            // Формирование итогового результата
            var result = posts
                .Where(post => channelsDict[post.ChannelId].LinkedChatId != null)
                .Select(post => new PostPreviewDto
                {
                    ChannelName = channelsDict[post.ChannelId].Title,
                    CommentCount = commentsCountDict.GetValueOrDefault((channelsDict[post.ChannelId].LinkedChatId ?? 0, post.InChatId), 0),
                    Post = post
                })
                .ToList();

            return result;
        }

        public async Task<Post> GetPostById(long id)
        {
            return await _mariaService.GetPostById(id);
        }

        public async Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId)
        {
            var post = await _mariaService.GetPostInChatById(postId, ChatId);
            post.InChatId = postIdInChat;
            await _mariaService.UpdatePostInChatId(post);
        }
    }
}