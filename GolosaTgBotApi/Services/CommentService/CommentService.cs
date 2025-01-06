
using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.ChannelService;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.PostService;
using GolosaTgBotApi.Services.UserService;
using System.Threading.Channels;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IMariaService _mariaService;
        private readonly IUserService _userService;
        private readonly IChannelService _channelService;
        private readonly IPostService _postService;
        private readonly ILogger<CommentService> _logger;

        private const int systemId = 777000;

        public CommentService(ILogger<CommentService> logger,
                              IMariaService mariadbService,
                              IUserService userService,
                              IChannelService channelService,
                              IPostService PostService)
        {
            _mariaService = mariadbService;
            _userService = userService;
            _channelService = channelService;
            _postService = PostService;
            _logger = logger;
        }
        public async Task HandleComment(Message message)
        {
            if (message.Chat.Id > 0)
            {
                return;
            }
            var comment = new Comment
            {
                TelegramId = message.Id,
                ParentId = message.ReplyToMessage?.MessageId,
                ChannelId = message.Chat.Id,
                MessageThreadId = message.MessageThreadId,
                UserId = message.From.Id,
                Text = message.Text,
                CreatedAt = DateTime.UtcNow
            };
            if (message.From.Id == systemId)
            {
                comment.IsPost = true;
                 await _postService.LinkPostAndMessage(message.ForwardFromMessageId, message.Id, message.Chat.Id);
            }
            try
            {
                // Retrieve user from the database
                var user = await _mariaService.GetUserbyIdAsync(message.From.Id);
                if (user == null)
                {
                    await _userService.CreateNewUser(message.From);
                }

                // Retrieve channel from the database
                await _channelService.CheckOnChannelExisting(message.Chat.Id);

                // Save the comment to the database
                await _mariaService.SaveCommentAsync(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message with ID {MessageId}", message.Id);
                // Handle the exception as appropriate
            }
        }
        private async Task HandleAutoComment(Comment comment)
        {
            

        }
    }

}
