namespace GolosaTgBotApi.Models.Dtos
{
    public class PostPreviewDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public string ChannelName { get; set; }
        public string ChannelAvatar {  get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<string> ImageUrls { get; set; } = Enumerable.Empty<string>();

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
