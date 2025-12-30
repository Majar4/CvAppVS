using System.ComponentModel.DataAnnotations;

namespace CvAppenVS.Web.Models
{
    public class RegistreraViewmodel
    {
        [Required(ErrorMessage = "Ange användarnamn.")]
        [StringLength(30, ErrorMessage = "Användarnamnet får max vara 30 tecken.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Skriv in lösenord.")]
        [DataType(DataType.Password)]
        

        public string Password { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenord.")]
        [DataType (DataType.Password)]
        [Display(Name  = "Bekräfta lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]

        public string BekraftaLosenord { get; set; }
    }
}
