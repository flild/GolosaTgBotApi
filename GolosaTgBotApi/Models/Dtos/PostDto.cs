namespace GolosaTgBotApi.Models.Dtos
{
    public class PostDto
    {
        public Post Post { get; set; }
        public int CommentCount {  get; set; }
        public Channel Channel { get; set; }
    }
}
