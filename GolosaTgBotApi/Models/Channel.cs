using System.ComponentModel.DataAnnotations;


namespace GolosaTgBotApi.Models
{
    public class Channel
    {
        [Key]
        public long Id { get; set; }
        public string Title {  get; set; }
        public long? OwnerId {  get; set; }
        public long? LinkedChatId { get; set; }
        public LinkedChat LinkedChat { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
