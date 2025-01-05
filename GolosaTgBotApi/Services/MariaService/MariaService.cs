
using GolosaTgBotApi.Data;
using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public class MariaService : IMariaService
    {
        private readonly MariaContext _db;

        public MariaService(MariaContext db)
        {
            _db = db;
        }
        public async void SaveCommentAsync(Message message)
        {
            var comment = new Comment();
            comment.Id = message.Id;
            //comment.PostId = message
            comment.UserId = message.From.Id;
            comment.Text = message.Text;
            await _db.AddAsync(comment);
            await _db.SaveChangesAsync(); 
            return;
        }
        public async Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId)
        {
            // Получить комментарий из определенного чата с Id
            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.ChanelId == chatId && c.TelegramId == commentTlgId);
            return comment;
        }
        public async Task<Models.User>? GetUserbyIdAsync(long userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
