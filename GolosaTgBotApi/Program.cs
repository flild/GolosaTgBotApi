using GolosaTgBotApi.Data;
using GolosaTgBotApi.Services.CommentService;
using GolosaTgBotApi.Services.MariaService;
using GolosaTgBotApi.Services.TelegramService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MariaContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        ServerVersion.Parse("11.4.4-mariadb"));
});
// Add services to the container.
builder.Services.AddSingleton(Channel.CreateUnbounded<Message>());
builder.Services.AddHostedService<TelegramService>();
builder.Services.AddHostedService<CommentService>();
builder.Services.AddScoped<IMariaService, MariaService>();
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


