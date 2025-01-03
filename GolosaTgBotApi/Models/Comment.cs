namespace GolosaTgBotApi.Models
{
    public class Comment
    {
        public long Id { get; set; } // Уникальный идентификатор комментария
        public int TelegramID { get; set; }
        public long? PostId { get; set; } // ID поста
        public long UserId { get; set; } // ID пользователя
        public long? ParentId { get; set; } // ID рrодительского комментария (если есть)
        public bool IsDelete { get; set; } = false;
        public long ChanelId { get; set; }
        public string Content { get; set; } // Текст комментария
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата создания

        // Навигационные свойства
        public ICollection<Comment> Replies { get; set; } // Ответы на комментарий
        public User User { get; set; }
        public Post Post { get; set; }
        public Comment Parent { get; set; }
    }
}
