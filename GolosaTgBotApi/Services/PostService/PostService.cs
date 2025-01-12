using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.ChannelService;
using GolosaTgBotApi.Services.MariaService;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.PostService
{
    public class PostService:IPostService
    {
        private readonly IMariaService _mariaService;
        private readonly IChannelService _channelService;

        public PostService(IMariaService mariaService, IChannelService channelService)
        {
            _mariaService = mariaService;
            _channelService = channelService;
        }
        public async Task HandlePost(Message post)
        {
            var newpost = new Post
            {
                postId = post.Id,
                InChatId = 0,
                Text = post.Text,
                ChannelId = post.Chat.Id
            };
            await _channelService.CheckOnChannelExisting(post.Chat.Id);
            try
            {
                await _mariaService.CreateNewPost(newpost);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public async Task<List<PostDto>> GetPosts(int limit, int offset)
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
                .Where(post => channelsDict[post.ChannelId].LinkedChat != null)
                .Select(post => new PostDto
                {
                    Channel = channelsDict[post.ChannelId],
                    CommentCount = commentsCountDict.GetValueOrDefault((channelsDict[post.ChannelId].LinkedChatId ?? 0, post.InChatId), 0),
                    Post = post
                })
                .ToList();

            return result;
        }

        public async Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId)
        {
            var post = await _mariaService.GetPostInChatById(postId, ChatId);
            post.InChatId = postIdInChat;
            await _mariaService.UpdatePostInChatId(post);
        }
    }

}
