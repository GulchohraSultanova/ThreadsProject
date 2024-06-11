namespace ThreadsProject.Bussiness.DTOs.UserDtos
{
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public string UserName { get; set; }
    
    }
}
