namespace CvAppen.Web.ViewModels
{
    public class MyProfileViewModel
    {
        public string Name { get; set; }
        public bool isPublic { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int CVId { get; set; }
        public int UnReadMessagesCount { get; set; }
        public List<MessageSummaryViewModel> RecentMessages { get; set; } = new List<MessageSummaryViewModel>();
        public List<ProjectSummaryViewModel> MyProjects { get; set; } = new List<ProjectSummaryViewModel>();
    }
    public class MessageSummaryViewModel
    {
        public int MessageId { get; set; }
        public string SenderName {  get; set; }
        public string Subject { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
    }
}
