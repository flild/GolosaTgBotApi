
using GolosaTgBotApi.Data;
using GolosaTgBotApi.Models;
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
            comment.Content = message.Text;
            await _db.AddAsync(comment);
            await _db.SaveChangesAsync(); 
            return;
        }

    }
}
