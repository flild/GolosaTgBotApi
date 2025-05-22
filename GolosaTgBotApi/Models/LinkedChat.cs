using System.ComponentModel.DataAnnotations;

namespace GolosaTgBotApi.Models
{
    public class LinkedChat
    {
        [Key]
        public long Id { get; set; }

        public string? Name { get; set; }
        public long? ChannelID { get; set; }
        public Channel Channel { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Comment> Comments { get; set; }
    }
}