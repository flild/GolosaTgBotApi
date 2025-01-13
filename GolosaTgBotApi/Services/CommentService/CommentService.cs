﻿
using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.ChannelService;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.PostService;
using GolosaTgBotApi.Services.TelegramService;
using GolosaTgBotApi.Services.UserService;
using System.Threading.Channels;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GolosaTgBotApi.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IMariaService _mariaService;
        private readonly IUserService _userService;
        private readonly IChannelService _channelService;
        private readonly ITelegramService _telegramService;
        private readonly IPostService _postService;
        private readonly ILogger<CommentService> _logger;

        private const int systemId = 777000;

        public CommentService(ILogger<CommentService> logger,
                              IMariaService mariadbService,
                              IUserService userService,
                              IChannelService channelService,
                              ITelegramService telegramService,
                              IPostService PostService)
        {
            _mariaService = mariadbService;
            _userService = userService;
            _channelService = channelService;
            _telegramService = telegramService;
            _postService = PostService;
            _logger = logger;
        }
        public async Task HandleComment(Message message)
        {
            if (message.Text.ToLower() == "/golosastart")
            {
                await HandleGolosaStartCommand(message);
                return;
            }
            if (message.Chat.Id > 0)
            {
                return;
            }
            var comment = new Comment
            {
                TelegramId = message.Id,
                ParentId = message.ReplyToMessage?.MessageId,
                ChatId = message.Chat.Id,
                MessageThreadId = message.MessageThreadId,
                UserId = message.From.Id,
                Text = message.Text,
                CreatedAt = DateTime.UtcNow

            };
            if (message.From.Id == systemId)
            {
                comment.IsPost = true;
                await _postService.LinkPostAndMessage(message.ForwardFromMessageId, message.Id, message.SenderChat.Id);
            }
            try
            {
                // Retrieve user from the database
                var user = await _mariaService.GetUserbyIdAsync(message.From.Id);
                if (user == null)
                {
                    await _userService.CreateNewUser(message.From);
                }

                // Save the comment to the database
                await _mariaService.SaveCommentAsync(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message with ID {MessageId}", message.Id);
                // Handle the exception as appropriate
            }
        }
        public async Task<List<Comment>> GetCommentsByPostId(long PostId, int limit, int offset)
        {
            var post = await _mariaService.GetPostById(PostId);
            var channel = await _mariaService.GetChannelById(post.ChannelId);
            var comments = await _mariaService.GetCommentsByTreadId(post.InChatId, channel.LinkedChatId??0, limit, offset);
            return comments;
        }
        private async Task HandleGolosaStartCommand(Message message)
        {
            if (message.Chat.Id >= 0)
            {
                // Not a channel chat
                return;
            }
/*       пока что это работает криво
        телеграм не всегда верно присылает данные об owner
 *       var isAdmin = await _telegramService.IsUserAdministrator(message.Chat.Id, message.From.Id);
            if (!isAdmin)
            {
                _telegramService.SendMessageInChat(message.Chat.Id, "Сообщение должен отправить администратор");
                return;
            }*/
            var chatInfo = await _telegramService.GetChatInfoById(message.Chat.Id);
            if(chatInfo.LinkedChatId == null)
            {
                _telegramService.SendMessageInChat(message.Chat.Id, "пока что бот работает только в чатах каналов");
                return;
            }

            try
            {
                await _channelService.CreateNewChannel(chatInfo.LinkedChatId??0);
                await _telegramService.SendMessageInChat(message.Chat.Id, "Чат зарегистрирован успешно");
            }
            catch (Exception ex)
            {
                // Handle exception
                await _telegramService.SendMessageInChat(message.Chat.Id, "Ошибка");
            }
        }
        
    }

}
