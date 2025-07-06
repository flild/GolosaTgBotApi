using GolosaTgBotApi.Models;
using GolosaTgBotApi.Models.Dtos;
using GolosaTgBotApi.Services.FileService;
using GolosaTgBotApi.Services.MariaService;

namespace GolosaTgBotApi.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMariaService _mariaService;
        private readonly IFileService _fileService;
        private readonly ILogger<PostService> _logger;

        public PostService(IMariaService mariaService, IFileService fileService, ILogger<PostService> logger)
        {
            _mariaService = mariaService;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<IEnumerable<PostPreviewDto>> GetPosts(int limit, int offset)
        {
            var posts = await _mariaService.GetLatestsPosts(limit, offset);

            // Уникальная структура для хранения (ChatId, ThreadId)
            var chatsOfThreadIds = new Dictionary<long, HashSet<int>>();
            foreach (var post in posts)
            {

                var chatId = post.Channel.LinkedChatId.Value;
                if (!chatsOfThreadIds.ContainsKey(chatId))
                    chatsOfThreadIds[chatId] = new HashSet<int>();

                chatsOfThreadIds[chatId].Add(post.InChatId);
            }

            // Получаем словарь с подсчётом комментариев
            var commentsCountDict = await _mariaService.GetCommentCountByIds(chatsOfThreadIds);

            // Формирование итогового результата
            var result = posts
                .Where(post => post.Channel.LinkedChatId != null)
                .Select(post =>
                {
                    // Берём список fileId из сущности
                    var fileIds = post.ImagesFileId ?? new List<string>();

                    // Конвертируем fileId в публичные URL
                    var urls =  fileIds
                        .Select(id => _fileService.GetOrDownloadAndGetImageUrlAsync(id).Result)
                        .ToList();

                    return new PostPreviewDto
                    {
                        Id = post.Id,
                        Text = post.Text,
                        ChannelName = post.Channel.Title,
                        ChannelAvatar = "",  //todo подставить аватар канала
                        CommentsCount = commentsCountDict.GetValueOrDefault((post.Channel.LinkedChatId!.Value, post.InChatId), 0),
                        CreatedAt = post.CreatedAt,
                        ImageUrls = urls
                    };
                })
                .ToList();

            return result;
        }

        public async Task<Post> GetPostById(long id)
        {
            return await _mariaService.GetPostById(id);
        }

        public async Task LinkPostAndMessage(int? postId, int postIdInChat, long ChatId)
        {
            if (postId == null)
            {
                // Не указан ID поста
                _logger.LogWarning("LinkPostAndMessage: postId is null for ChatId {ChatId}", ChatId);
                return;
            }
            var post = await _mariaService.GetPostInChatById(postId, ChatId);
            if (post == null)
            {
                _logger.LogWarning("LinkPostAndMessage: Post not found for postId {PostId} in ChatId {ChatId}", postId, ChatId);
                return;
            }

            post.InChatId = postIdInChat;
            await _mariaService.UpdatePostInChatId(post);
        }
    }
}