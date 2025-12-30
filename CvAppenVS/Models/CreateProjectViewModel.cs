using System.ComponentModel.DataAnnotations;

namespace CvAppenVS.Web.Models
{
    public class CreateProjectViewModel
    {
        [Required(ErrorMessage = "Ange projektets titel.")]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
