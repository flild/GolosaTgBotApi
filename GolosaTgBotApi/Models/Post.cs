using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GolosaTgBotApi.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; } // Уникальный идентификатор поста

        public List<string>? ImagesFileId { get; set; }
        public int postId { get; set; }
        public long? MediaGroup { get; set; }
        public int InChatId { get; set; }
        public string? Text { get; set; } // Текст поста
        public bool IsDelete { get; set; } = false;
        public long ChannelId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата публикации

        [NotMapped]
        public Channel Channel { get; set; }
    }
}