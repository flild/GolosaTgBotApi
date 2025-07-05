namespace GolosaTgBotApi.Models.Dtos
{
    public class PostPreviewDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostDto : PostPreviewDto
    {
        public string Content { get; set; }
    }

    public class CommentDto
    {
        public long Id { get; set; }
        public long PostId { get; set; }
        public long? ParentCommentId { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
