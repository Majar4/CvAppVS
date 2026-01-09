using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string CurrentPassword { get; set;  }

        [Required]
        [MinLength(6, ErrorMessage = "Lösenordet måste innehålla minst 6 tecken.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Lösenordet måste innehålla minst en stor bokstav och en siffra.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        public string ConfirmPassword { get; set; }
    }
}
