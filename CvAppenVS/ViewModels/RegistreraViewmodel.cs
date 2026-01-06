using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.ViewModels
{
    public class RegistreraViewmodel
    {
        [Required(ErrorMessage = "Ange användarnamn.")]
        [StringLength(30, ErrorMessage = "Användarnamnet får max vara 30 tecken.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Ange namn.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Ange adress.")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Ladda upp en bild.")]
        public required IFormFile Image { get; set; }

        [Required(ErrorMessage = "Skriv in lösenord.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one uppercase letter and one number.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenord.")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public required string BekraftaLosenord { get; set; }
    }
}
