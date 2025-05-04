using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Threading.Channels;
using DotNetEnv;
using GolosaTgBotApi.Models;

namespace GolosaTgBotApi.Services.TelegramService
{
    public class TelegramBgService : BackgroundService
    {
        private readonly ITelegramBotClient bot;
        private readonly Channel<Update> _updateChannel;

        public TelegramBgService(Channel<Update> updateChannel)
        {
            Env.Load();
            Console.WriteLine(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"));
            bot = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_BOT_API_KEY"));
            _updateChannel = updateChannel;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(update));

            //await _updateChannel.Writer.WriteAsync(update);
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