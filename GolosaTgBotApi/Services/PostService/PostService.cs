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
        public async Task<PostWithCommentsDto> GetPostWithComments(long postId, int commentCount)
        {
            var post  = await _mariaService.GetPostById(postId);
            //как чат то узнать?
            //var comments = await _mariaService.GetCommentsByTreadId(post.InChatId,post.)
            return new PostWithCommentsDto();
        }

        public async Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId)
        {
            var post = await _mariaService.GetPostInChatById(postId, ChatId);
            post.InChatId = postIdInChat;
            await _mariaService.UpdatePostInChatId(post);
        }
    }

}
