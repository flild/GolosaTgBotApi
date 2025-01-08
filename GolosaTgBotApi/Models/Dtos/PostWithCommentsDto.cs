namespace GolosaTgBotApi.Models.Dtos
{
    public class PostWithCommentsDto
    {
        public Post Post { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
