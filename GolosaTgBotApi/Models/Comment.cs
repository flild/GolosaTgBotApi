using System.ComponentModel.DataAnnotations;

namespace GolosaTgBotApi.Models
{
    public class Comment
    {
        // Идентификаторы
        [Key]
        public long Id { get; set; } // Уникальный идентификатор комментария
        public int TelegramId { get; set; }
        public int? ParentId { get; set; } // ID родительского комментария (если есть)
        public int? MessageThreadId { get; set; }
        public long ChatId { get; set; }

        // Данные о пользователе
        public long UserId { get; set; } // ID пользователя

        // Флаги
        public bool IsPost { get; set; } = false;
        public bool IsDelete { get; set; } = false;

        // Содержимое
        public string Text { get; set; } // Текст комментария

        // Временные метки
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания

        // Навигационные свойства
        public ICollection<Comment> Replies { get; set; } // Ответы на комментарий
        public User User { get; set; }
        public Comment Parent { get; set; }
        public LinkedChat LinkedChat { get; set; }
    }
}