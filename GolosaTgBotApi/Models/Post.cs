using System.ComponentModel.DataAnnotations;

namespace GolosaTgBotApi.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; } // Уникальный идентификатор поста
        public int postId { get; set; }
        public int InChatId { get; set; }
        public string? Text { get; set; } // Текст поста
        public bool IsDelete { get; set; } = false;
        public long ChannelId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата публикации

    }
}
