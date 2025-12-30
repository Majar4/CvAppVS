using System.ComponentModel.DataAnnotations;

namespace CvAppenVS.Web.Models
{
    public class CreateProjectViewModel
    {
        [Required(ErrorMessage = "Ange projektets titel.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Ange projektets beskrivning.")]
        public string Description { get; set; } = null!;

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
