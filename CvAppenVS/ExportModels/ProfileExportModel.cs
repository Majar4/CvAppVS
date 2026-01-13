namespace CvAppen.Web.ExportModels
{
    public class ProfileExportModel
    {
        public string Name { get; set; }
        public string Email { get; set; }   
        public string Address { get; set; }
        public bool IsPrivate { get; set; }

        public CvExportModel CV {  get; set; }
        public List<ProjectExportModel> Projects { get; set; } = new(); 
    }
}
