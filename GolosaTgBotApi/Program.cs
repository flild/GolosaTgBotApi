using GolosaTgBotApi.Data;
using GolosaTgBotApi.Services.CommentService;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using GolosaTgBotApi.Services.UserService;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using DotNetEnv;
using GolosaTgBotApi.Services.ChannelService;
using EntityChannel = System.Threading.Channels.Channel;
using GolosaTgBotApi.Services.PostService;
using GolosaTgBotApi.Services.MessageHandlerService;

var builder = WebApplication.CreateBuilder(args);
Env.Load();
builder.Services.AddDbContext<MariaContext>(options =>
{
    options.UseMySql(
        Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"),
        ServerVersion.Parse("11.4.4-mariadb"));
});
// Add services to the container.
builder.Services.AddSingleton(EntityChannel.CreateUnbounded<Update>());
builder.Services.AddHostedService<TelegramBgService>();
builder.Services.AddHostedService<UpdateHandlerService>();
builder.Services.AddScoped<ICommentService,CommentService>();
builder.Services.AddScoped<IMariaService, MariaService>();
builder.Services.AddScoped<IPostHandleService, PostHandleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


