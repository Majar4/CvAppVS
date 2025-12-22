namespace CvAppenVS.Models
{
    public class Message
    {
        public int Id { get; set; } 
        public string Text {  get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; } 
        public bool IsRead { get; set; }
        public DateTime SentAt {  get; set; }
    }
}
