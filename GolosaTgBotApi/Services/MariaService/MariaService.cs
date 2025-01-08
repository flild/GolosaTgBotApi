
using GolosaTgBotApi.Data;
using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
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
        public async Task SaveCommentAsync(Comment comment)
        {
            await _db.AddAsync(comment);
            await _db.SaveChangesAsync(); 
            return;
        }
        public async Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId)
        {
            // Получить комментарий из определенного чата с Id
            var comment = await _db.Comments
                .FirstOrDefaultAsync(c => c.ChatId == chatId && c.TelegramId == commentTlgId);
            return comment;
        }
        public async Task<List<Comment>> GetCommentsByTreadId(int treadId, long chatId, int limit, int offset)
        {
            var comments = await _db.Comments
                .Where(c => c.MessageThreadId == treadId && c.ChatId == chatId && !c.IsPost)
                .OrderBy(c => c.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
            return comments;
        }
        public async Task<Models.User?> GetUserbyIdAsync(long userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task CreateNewUser(Models.User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        public async Task<Channel?> GetChannelById(long ChannelId)
        {
            return await _db.Channels.FirstOrDefaultAsync(c => c.Id == ChannelId);
        }
        public async Task CreateNewChannel(Channel channel)
        {
            _db.Channels.Add(channel);
            await _db.SaveChangesAsync();
        }
        public async Task<Post> GetPostInChatById(int? PostId, long ChatId)
        {
            return await _db.Posts.FirstOrDefaultAsync(p => p.postId == PostId && p.ChannelId == ChatId);
        }
        public async Task CreateNewPost(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
        }
        public async Task<Post> GetPostById(long id)
        {
            return await _db.Posts.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task UpdatePostInChatId(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }
    }
}
