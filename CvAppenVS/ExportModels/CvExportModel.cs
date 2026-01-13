namespace CvAppen.Web.ExportModels
{
    public class CvExportModel
    {
        public string Presentation { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Competences { get; set; } = new(); //endast ett fält
        
        public List<EducationExportModel> Educations { get; set; } = new();
        public List<ExperienceExportModel> Experiences { get; set; } = new();
    }
}
