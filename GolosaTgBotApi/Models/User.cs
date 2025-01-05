namespace GolosaTgBotApi.Models
{
    public class User
    {
        public long Id { get; set; } // Уникальный идентификатор пользователя
        public string Username { get; set; } // Имя пользователя
        public string AvatarUrl { get; set; } // URL аватарки пользователя

        // Связь с комментариями
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Channel> chanels { get; set; }
    }
}
