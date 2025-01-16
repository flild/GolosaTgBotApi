using GolosaTgBotApi.Services.CommentService;
using GolosaTgBotApi.Services.PostService;
using System.Threading.Channels;
using Telegram.Bot.Types;
using updateTypes = Telegram.Bot.Types.Enums.UpdateType;

namespace GolosaTgBotApi.Services.MessageHandlerService
{
    public class UpdateHandlerService : BackgroundService
    {
        private readonly Channel<Update> _channel;
        private readonly IServiceProvider _serviceProvider;

        public UpdateHandlerService(Channel<Update> channel, IServiceProvider serviceProvider)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Чтение сообщений из канала в бесконечном цикле
            await foreach (var updateEvent in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = _serviceProvider.CreateScope();
                {
                    if (updateEvent.Type == updateTypes.ChannelPost)
                    {
                        var postService = scope.ServiceProvider.GetRequiredService<IPostHandleService>();
                        await postService.HandlePost(updateEvent.ChannelPost);
                    }
                    //если id отрицательный, то это коммент в чате
                    if (updateEvent.Type == updateTypes.Message && updateEvent.Message.Chat.Id < 0)
                    {
                        var commentService = scope.ServiceProvider.GetRequiredService<ICommentService>();
                        await commentService.HandleComment(updateEvent.Message);
                    }
                }
            }
        }
    }
}