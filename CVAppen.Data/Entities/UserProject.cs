namespace CvAppen.Data
{
    public class UserProject
    {
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public int ProjectId { get; set; }  
        public Project Project { get; set; } = null!;
    }
}
