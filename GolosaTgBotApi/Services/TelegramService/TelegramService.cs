using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Threading.Channels;
using DotNetEnv;

namespace GolosaTgBotApi.Services.TelegramService
{
    public class TelegramService: BackgroundService
    {
        private readonly ITelegramBotClient bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"));
        private readonly Channel<Message> _messageChannel;

        public TelegramService(Channel<Message> messageChannel)
        {
            Env.Load();
            _messageChannel = messageChannel;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                await _messageChannel.Writer.WriteAsync(update.Message);
            }
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.ChannelPost)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(JsonConvert.SerializeObject(update));
            }
            
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(JsonConvert.SerializeObject(exception));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Запущен бот " + bot.GetMe().Result.FirstName);
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
        }
    }
}
