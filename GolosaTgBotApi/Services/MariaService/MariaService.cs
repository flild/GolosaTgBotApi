﻿
using GolosaTgBotApi.Data;
using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<Dictionary<(long ChatId, int ThreadId), int>> GetCommentCountByIds(Dictionary<long, HashSet<int>> chatsOfThreadIds)
        {
            // Step 1: Extract chat IDs and use the original dictionary for thread IDs
            var chatIds = chatsOfThreadIds.Keys.ToList();
            var chatIdToThreadIds = chatsOfThreadIds;

            // Step 2: Retrieve all relevant comments from the database
            var allComments = await _db.Comments
                .Where(c => chatIds.Contains(c.ChatId) && c.MessageThreadId.HasValue)
                .ToListAsync();

            // Step 3: Filter comments in memory based on thread IDs
            var filteredComments = allComments
                .Where(c => chatIdToThreadIds.ContainsKey(c.ChatId) && chatIdToThreadIds[c.ChatId].Contains(c.MessageThreadId.Value))
                .ToList();

            // Step 4: Group and count the filtered comments
            var commentCounts = filteredComments
                .GroupBy(c => (c.ChatId, c.MessageThreadId.Value))
                .ToDictionary(g => g.Key, g => g.Count());

            // Step 5: Return the result
            return commentCounts;
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
        public async Task<List<Post>> GetLatestsPosts(int limit, int offset)
        {
            return await _db.Posts
                .OrderByDescending(p => p.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        public async Task UpdatePostInChatId(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }
    }
}
