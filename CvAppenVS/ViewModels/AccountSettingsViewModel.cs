using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.ViewModels
{
    public class AccountSettingsViewModel
    {
        [Required(ErrorMessage = "Vänligen ange ett fullständigt namn")]
        [StringLength(50, MinimumLength = 2, ErrorMessage="Namnet måste vara mellan 2 och 50 tecken")]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage="Adressen får inte vara över 100 tecken")]
        [Display(Name = "Adress")]
        public string Address { get; set; }

        [Display(Name = "Privat profil")]
        public bool IsPrivate { get; set;  }
    }
}
