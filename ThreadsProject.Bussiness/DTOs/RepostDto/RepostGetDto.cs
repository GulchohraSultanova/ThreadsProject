using ThreadsProject.Bussiness.DTOs.PostDto;

namespace ThreadsProject.Bussiness.DTOs.RepostDto
{
    public class RepostGetDto
    {
        public int Id { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public PostGetDto Post { get; set; }
    }
}
