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
            var Channels = await _mariaService.GetChannelsByIds(posts.Select(p => p.ChannelId).ToList());
            var chatsOfThreadIds = new Dictionary<long, List<int>>();
            foreach (var post in posts) 
            {
                var chatId = Channels.First(ch => ch.Id == post.ChannelId).LinkedChatId??0;
                if (chatId == 0)
                    continue;
                if (!chatsOfThreadIds.ContainsKey(chatId))
                {
                    chatsOfThreadIds[chatId] = new List<int>();
                }
                chatsOfThreadIds[chatId].Add(post.InChatId);
            }
            var CommentsCountDict = await _mariaService.GetCommentCountByIds(chatsOfThreadIds);
            var result = new List<PostDto>();
            foreach (var post in posts)
            {
                var channel = Channels.First(ch => ch.Id == post.ChannelId);
                if (channel.LinkedChat == null)
                    continue;
                var postDto = new PostDto
                {
                    Channel = channel,
                    CommentCount = CommentsCountDict[post.InChatId],
                    Post = post,
                };
                result.Add(postDto);
            }


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
