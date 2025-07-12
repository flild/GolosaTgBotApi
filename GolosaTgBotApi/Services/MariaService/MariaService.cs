using GolosaTgBotApi.Data;
using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GolosaTgBotApi.Services.MariaService
{
    public class MariaService : IMariaService
    {
        private readonly MariaContext _db;

        public MariaService(MariaContext db)
        {
            _db = db;
        }

        #region Comments

        public async Task SaveCommentAsync(Comment comment)
        {
            if (comment.ParentId != null)
            {
                var parentComment = await _db.Comments
                    .FirstOrDefaultAsync(c => c.TelegramId == comment.ParentId && c.ChatId == comment.ChatId);
                if (parentComment == null)
                {
                    comment.ParentId = null;
                    comment.Parent = null;
                }
            }
            await _db.AddAsync(comment);
            await _db.SaveChangesAsync();
            return;
        }

        public async Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId)
        {
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

        public async Task<Dictionary<(long ChatId, int ThreadId), int>> GetCommentCountByIds(Dictionary<long, HashSet<int>> chatsOfThreadIds)
        {
            var chatIds = chatsOfThreadIds.Keys.ToList();
            var chatIdToThreadIds = chatsOfThreadIds;
            var allComments = await _db.Comments
                .Where(c => chatIds.Contains(c.ChatId) && c.MessageThreadId.HasValue)
                .ToListAsync();
            var filteredComments = allComments
                .Where(c => chatIdToThreadIds.ContainsKey(c.ChatId) && chatIdToThreadIds[c.ChatId].Contains(c.MessageThreadId.Value))
                .ToList();
            var commentCounts = filteredComments
                .GroupBy(c => (c.ChatId, c.MessageThreadId.Value))
                .ToDictionary(g => g.Key, g => g.Count());
            return commentCounts;
        }

        public async Task<Comment?> GetCommentByMediaGroupAsync(long mediaGroup, long chatId)
        {
            return await _db.Comments
                .FirstOrDefaultAsync(p => p.MediaGroup == mediaGroup && p.ChatId == chatId);
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
        }

        #endregion Comments

        #region User

        public async Task<Models.User?> GetUserbyIdAsync(long userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task CreateNewUser(Models.User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        #endregion User

        #region Channel

        public async Task<Channel?> GetChannelById(long ChannelId)
        {
            return await _db.Channels.Include(c => c.LinkedChat).FirstOrDefaultAsync(c => c.Id == ChannelId);
        }

        public async Task<List<Channel>> GetChannelsByIds(List<long> channelIds)
        {
            return await _db.Channels
                .Where(c => channelIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task CreateNewChannel(Channel channel)
        {
            _db.Channels.Add(channel);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateChannelInfo(Channel channel)
        {
            if (channel.LinkedChat != null)
            {
                _db.LinkedChats.Add(channel.LinkedChat);
            }
            _db.Channels.Update(channel);

            await _db.SaveChangesAsync();
        }

        #endregion Channel

        #region Post

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
            return await _db.Posts.Include(p => p.Channel).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Post>> GetLatestsPosts(int limit, int offset)
        {
            return await _db.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .Include(p => p.Channel)
                .ToListAsync();
        }

        public async Task UpdatePostInChatId(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }

        public async Task<Post?> GetPostByMediaGroupAsync(long mediaGroup, long channelId)
        {
            return await _db.Posts
                .FirstOrDefaultAsync(p => p.MediaGroup == mediaGroup && p.ChannelId == channelId);
        }

        public async Task CreateNewPostAsync(Post post)
        {
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
        }

        public async Task UpdatePostAsync(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }

        #endregion Post

        #region File

        /// <inheritdoc />
        public DownloadedFile? GetDownloadedFile(string fileId)
        {
            // Ищем запись по fileId. Если найдена, возвращаем FilePath, иначе null.
            var record = _db.FileRecords
                            .AsNoTracking()
                            .FirstOrDefault(f => f.FileId == fileId);
            return record;
        }

        /// <inheritdoc />
        public void DeleteFile(string fileId)
        {
            // Находим запись по fileId. Если найдена, удаляем и сохраняем изменения.
            var record = _db.FileRecords.FirstOrDefault(f => f.FileId == fileId);
            if (record != null)
            {
                _db.FileRecords.Remove(record);
                _db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void UpdateFile(DownloadedFile fileRecord)
        {
            // Помечаем объект как модифицированный и сохраняем.
            _db.FileRecords.Update(fileRecord);
            _db.SaveChanges();
        }

        /// <inheritdoc />
        public void AddFile(DownloadedFile newFileRecord)
        {
            _db.FileRecords.Add(newFileRecord);
            _db.SaveChanges();
        }

        #endregion
    }
}