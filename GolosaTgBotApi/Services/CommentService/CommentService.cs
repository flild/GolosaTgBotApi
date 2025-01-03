
using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.MariaService;
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
                    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(message));
                    if(message.Chat.Id < 0)
                    {
                        var comment = new Comment();
                        comment.TelegramID = message.Id;
                        comment.PostId = message.MessageThreadId;
                    }
                    // Обработка сообщения и сохранение в базу данных
                    // await mariadbService.SaveCommentAsync(message);
                }
            }
        }
    }
}
