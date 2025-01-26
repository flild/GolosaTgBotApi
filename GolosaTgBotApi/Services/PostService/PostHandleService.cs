using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.ChannelService;
using GolosaTgBotApi.Services.MariaService;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.PostService
{
    public class PostHandleService : IPostHandleService
    {
        private readonly IChannelService _channelService;
        private readonly IMariaService _mariaService;

        public PostHandleService(IChannelService channelService, IMariaService mariaService)
        {
            _channelService = channelService;
            _mariaService = mariaService;
        }

        public async Task HandlePost(Message post)
        {
            var newpost = new Post();
            newpost.postId = post.Id;
            newpost.InChatId = 0;
            newpost.Text = post.Text;
            newpost.ChannelId = post.Chat.Id;
            //newpost.PhotosFileId = post.Photo[0].FileId;
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
    }
}