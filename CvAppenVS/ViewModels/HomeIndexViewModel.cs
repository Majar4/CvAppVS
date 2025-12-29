namespace CvAppen.Web.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<CvSummaryViewModel> FeaturedCvs { get; set; } = new List<CvSummaryViewModel>();
        public ProjectSummaryViewModel LatestProject { get; set; }
    }
    //Hjälpklasser för att hålla data
    public class CvSummaryViewModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Presentation { get; set; }
    }
    public class ProjectSummaryViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
