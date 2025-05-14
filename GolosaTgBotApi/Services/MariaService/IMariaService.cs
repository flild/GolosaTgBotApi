using GolosaTgBotApi.Models;
using Telegram.Bot.Types;

namespace GolosaTgBotApi.Services.MariaService
{
    public interface IMariaService
    {
        #region Comments

        /// <summary>
        /// Сохраняет комментарий в базе данных
        /// </summary>
        Task SaveCommentAsync(Comment comment);

        /// <summary>
        /// Получает комментарий по идентификатору чата и телеграм ID комментария
        /// </summary>
        Task<Comment>? GetCommentFromChat(long chatId, int commentTlgId);

        /// <summary>
        /// Получает список комментариев по идентификатору треда с пагинацией
        /// </summary>
        Task<List<Comment>> GetCommentsByTreadId(int treadId, long chatId, int limit, int offset);

        /// <summary>
        /// Возвращает количество комментариев для указанных чатов и тредов
        /// </summary>
        Task<Dictionary<(long ChatId, int ThreadId), int>> GetCommentCountByIds(Dictionary<long, HashSet<int>> chatsOfThreadIds);

        Task<Comment?> GetCommentByMediaGroupAsync(long mediaGroup, long chatId);

        Task UpdateCommentAsync(Comment comment);

        #endregion Comments

        #region User

        /// <summary>
        /// Получает пользователя по его ID
        /// </summary>
        Task<Models.User?> GetUserbyIdAsync(long userId);

        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        Task CreateNewUser(Models.User user);

        #endregion User

        #region Channel

        /// <summary>
        /// Получает канал по его ID
        /// </summary>
        Task<Channel?> GetChannelById(long ChannelId);

        /// <summary>
        /// Получает список каналов по списку ID
        /// </summary>
        Task<List<Channel>> GetChannelsByIds(List<long> channelIds);

        /// <summary>
        /// Создает новый канал
        /// </summary>
        Task CreateNewChannel(Channel channel);

        /// <summary>
        /// Обновляет информацию о канале
        /// </summary>
        Task UpdateChannelInfo(Channel channel);

        #endregion Channel

        #region Post

        /// <summary>
        /// Получает пост по ID внутри чата
        /// </summary>
        Task<Post> GetPostInChatById(int? PostId, long ChatId);

        /// <summary>
        /// Создает новый пост
        /// </summary>
        Task CreateNewPost(Post post);

        /// <summary>
        /// Получает пост по его внутреннему ID
        /// </summary>
        Task<Post> GetPostById(long id);

        /// <summary>
        /// Получает последние посты с пагинацией
        /// </summary>
        Task<List<Post>> GetLatestsPosts(int limit, int offset);

        /// <summary>
        /// Обновляет информацию о посте в чате
        /// </summary>
        Task UpdatePostInChatId(Post post);

        /// <summary>
        /// Получает пост по медиа-группе и ID канала
        /// </summary>
        Task<Post?> GetPostByMediaGroupAsync(long mediaGroup, long channelId);

        /// <summary>
        /// Асинхронно создает новый пост
        /// </summary>
        Task CreateNewPostAsync(Post post);

        /// <summary>
        /// Обновляет информацию о посте
        /// </summary>
        Task UpdatePostAsync(Post post);

        #endregion Post
    }
}