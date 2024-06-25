namespace ThreadsProject.Bussiness.DTOs.CommentDto
{
    public class CommentGetDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> LikeUserIds { get; set; } = new List<string>();
    }
}
