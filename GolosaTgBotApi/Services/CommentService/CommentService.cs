
using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.UserService;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.CommentService
{
    public class CommentService : BackgroundService
    {
        private readonly Channel<Message> _channel;
        private readonly IServiceProvider _serviceProvider;

        public CommentService(Channel<Message> channel, IServiceProvider serviceProvider)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            {
                // Чтение сообщений из канала в бесконечном цикле
                await foreach (var message in _channel.Reader.ReadAllAsync(stoppingToken))
                {

                    using var scope = _serviceProvider.CreateScope();
                    var mariadbService = scope.ServiceProvider.GetRequiredService<IMariaService>();
                    var userService = scope.ServiceProvider.GetRequiredService<IUserService>;
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                    if (message.Chat.Id > 0)
                    {
                        return;
                    }
                    var comment = new Comment();
                    //ids
                    comment.TelegramId = message.Id;
                    comment.IsPost = message.From.Id == 777000;
                    comment.ParentId = message.ReplyToMessage?.MessageId;
                    comment.ChanelId = message.SenderChat.Id;
                    comment.MessageThreadId = message.MessageThreadId;
                    comment.UserId = message.From.Id;
                    //content
                    comment.Text = message.Text;
                    comment.CreatedAt = DateTime.Now;
                    var user = mariadbService.GetUserbyIdAsync(message.From.Id);
                    if(user == null)
                    {
                        userService.CreateNewUser();

                    }
                    // Обработка сообщения и сохранение в базу данных
                    // await mariadbService.SaveCommentAsync(message);
                }
            }
        }
    }
}
