using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GolosaTgBotApi.Data
{
    public class MariaContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public MariaContext(DbContextOptions<MariaContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи комментариев с родительскими комментариями
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent) // Указание на отсутствие явного навигационного свойства
                .WithMany(c => c.Replies) // Связь с коллекцией дочерних комментариев
                .HasForeignKey(c => c.ParentId) // Внешний ключ для связи
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем каскадное удаление

            // Настройка связи комментариев и пользователей
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи комментариев и постов
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.Id);
            modelBuilder.Entity<Post>()
               .HasIndex(c => c.Id);
            modelBuilder.Entity<User>()
               .HasIndex(c => c.Id);
        }
    }
}
