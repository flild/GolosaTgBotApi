﻿namespace GolosaTgBotApi.Models
{
    public class Post
    {
        public long Id { get; set; } // Уникальный идентификатор поста
        public int InChatId { get; set; }
        public string Content { get; set; } // Текст поста
        public bool IsDelete { get; set; } = false;
        //todo нужно реверсивно проставлять, когда приходит соо в чате о посте
        public long ChanelId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата публикации

        // Связь с комментариями
        public ICollection<Comment> Comments { get; set; }
    }
}
