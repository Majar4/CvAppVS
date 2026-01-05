using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.ViewModels
{
    public class AccountSettingsViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsPrivate { get; set;  }
    }
}
