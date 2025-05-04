using GolosaTgBotApi.Models;
using GolosaTgBotApi.Services.ChannelService;
using GolosaTgBotApi.Services.MariaService;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.PostService
{
    public class PostHandleService : IPostHandleService
    {
        private readonly IChannelService _channelService;
        private readonly IMariaService _mariaService;

        public PostHandleService(IChannelService channelService, IMariaService mariaService)
        {
            _channelService = channelService;
            _mariaService = mariaService;
        }

        public async Task HandlePost(Message post)
        {
            // Ensure channel is registered
            await _channelService.CheckOnChannelExisting(post.Chat.Id);

            // If this is part of a media group
            if (!string.IsNullOrEmpty(post.MediaGroupId))
            {
                long mgid = long.Parse(post.MediaGroupId!);
                // Try to get existing post by media group
                var existingPost = await _mariaService.GetPostByMediaGroupAsync(mgid, post.Chat.Id);
                string fileId = post.Photo?.Last().FileId;

                if (existingPost != null)
                {
                    // Append new image fileId
                    existingPost.ImagesFileId ??= new List<string>();
                    existingPost.ImagesFileId.Add(fileId);
                    await _mariaService.UpdatePostAsync(existingPost);
                }
                else
                {
                    // First image in group: create new post record
                    var newPost = new Post
                    {
                        postId = post.MessageId,
                        InChatId = 0,
                        ChannelId = post.Chat.Id,
                        MediaGroup = mgid,
                        ImagesFileId = new List<string> { fileId },
                        Text = post.Caption ?? post.Text
                    };
                    await _mariaService.CreateNewPostAsync(newPost);
                }
            }
            else if (post.Photo != null && post.Photo.Any())
            {
                // Single photo
                string fileId = post.Photo.Last().FileId;
                var newPost = new Post
                {
                    postId = post.MessageId,
                    InChatId = 0,
                    ChannelId = post.Chat.Id,
                    ImagesFileId = new List<string> { fileId },
                    Text = post.Caption ?? post.Text
                };
                await _mariaService.CreateNewPostAsync(newPost);
            }
            else
            {
                // Pure text post
                var newPost = new Post
                {
                    postId = post.MessageId,
                    InChatId = 0,
                    ChannelId = post.Chat.Id,
                    Text = post.Text
                };
                await _mariaService.CreateNewPostAsync(newPost);
            }
        }
    }
}