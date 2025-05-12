using GolosaTgBotApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GolosaTgBotApi.Data
{
    public class MariaContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<LinkedChat> LinkedChats { get; set; }

        public MariaContext(DbContextOptions<MariaContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи комментариев с родительскими комментариями
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent) // Указываем навигационное свойство для родителя
                .WithMany(c => c.Replies) // Указываем навигационное свойство для дочерних комментариев
                .HasForeignKey(c => new { c.ParentId, c.ChatId }) // Составной внешний ключ (ParentId + ChannelId)
                .IsRequired(false)
                .HasPrincipalKey(c => new { c.TelegramId, c.ChatId }) // Составной ключ для связи (TelegramId + ChannelId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связи комментариев и пользователей
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            //связь комментов и чата
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.LinkedChat)
                .WithMany(ch => ch.Comments)
                .HasForeignKey(ch => ch.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            //связь канала и чата фк в чате
            modelBuilder.Entity<Channel>()
                .HasOne(c => c.LinkedChat)
                .WithOne(lc => lc.Channel)
                .HasForeignKey<LinkedChat>(lc => lc.ChannelID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LinkedChat>().ToTable("linkedchat");

            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.Id);
            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.MessageThreadId);
            modelBuilder.Entity<Comment>()
                .HasIndex(c => c.ChatId);
            modelBuilder.Entity<Post>()
               .HasIndex(c => c.Id);
            modelBuilder.Entity<User>()
               .HasIndex(c => c.Id);
            modelBuilder.Entity<Channel>()
               .HasIndex(c => c.Id);
        }
    }
}