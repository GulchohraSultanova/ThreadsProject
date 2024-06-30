using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadsProject.Bussiness.DTOs.CommentDto;

namespace ThreadsProject.Bussiness.DTOs.PostDto
{
    public  class PostGetDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
        public string UserImgUrl { get; set; }
        public string UserId { get; set; }
        public bool IsVerified { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Likes { get; set; } = new List<string>();
        public string time => CreatedDate.ToRelativeTime();


        public List<CommentGetDto> Comments { get; set; } = new List<CommentGetDto>();

    }
    
  public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan <= TimeSpan.FromSeconds(60))
                return "now";

            if (timeSpan <= TimeSpan.FromMinutes(60))
                return timeSpan.Minutes > 1 ? $"{timeSpan.Minutes}m" : "1m";

            if (timeSpan <= TimeSpan.FromHours(24))
                return timeSpan.Hours > 1 ? $"{timeSpan.Hours}h" : "1h";

            if (timeSpan <= TimeSpan.FromDays(7))
                return timeSpan.Days > 1 ? $"{timeSpan.Days}d" : "1d";

            if (timeSpan <= TimeSpan.FromDays(30))
                return timeSpan.Days > 7 ? $"{timeSpan.Days / 7}w" : "1w";

            if (timeSpan <= TimeSpan.FromDays(365))
                return timeSpan.Days > 30 ? $"{timeSpan.Days / 30}mo" : "1mo";

            return dateTime.ToString("yyyy-MM-dd");
        }
    }


}
