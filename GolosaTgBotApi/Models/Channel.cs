using System.ComponentModel.DataAnnotations;

namespace GolosaTgBotApi.Models
{
    public class Channel
    {
        [Key]
        public long Id { get; set; }
        public long? OwnerId {  get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
