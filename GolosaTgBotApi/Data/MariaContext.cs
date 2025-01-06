using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GolosaTgBotApi.Data
{
    public class MariaContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public MariaContext(DbContextOptions<MariaContext> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи комментариев с родительскими комментариями
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent) // Указываем навигационное свойство для родителя
                .WithMany(c => c.Replies) // Указываем навигационное свойство для дочерних комментариев
                .HasForeignKey(c => new { c.ParentId, c.ChannelId }) // Составной внешний ключ (ParentId + ChannelId)
                .HasPrincipalKey(c => new { c.TelegramId, c.ChannelId }) // Составной ключ для связи (TelegramId + ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи комментариев и пользователей
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.Id);
            modelBuilder.Entity<Post>()
               .HasIndex(c => c.Id);
            modelBuilder.Entity<User>()
               .HasIndex(c => c.Id);
            modelBuilder.Entity<Channel>()
               .HasIndex(c => c.Id);
        }
    }
}
