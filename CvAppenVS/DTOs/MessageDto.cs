using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string Text { get; set; }
        
        public string? SenderName { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}
