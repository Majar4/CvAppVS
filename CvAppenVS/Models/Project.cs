namespace CvAppenVS.Models
{
    public class Project
    {
        public int Id { get; set; } 
        public string Title { get; set; }    
        public string Description { get; set; }
        public DateTime Date { get; set; }

        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}
