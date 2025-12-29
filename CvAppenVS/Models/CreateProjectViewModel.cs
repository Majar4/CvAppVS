using System.ComponentModel.DataAnnotations;

namespace CvAppenVS.Models
{
    public class CreateProjectViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
