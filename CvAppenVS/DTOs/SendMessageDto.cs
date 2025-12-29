namespace CvAppen.Web.DTOs
{
    public class SendMessageDto
    {
        public string ToUserId { get; set; } = null!;
        public string Text { get; set; } = null!;
        public string? SenderName { get; set; }

    }
}
