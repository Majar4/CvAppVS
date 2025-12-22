using System.ComponentModel.DataAnnotations;

namespace CvAppenVS.Models


{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ange användarnamn.")]
        [StringLength(255)]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Ange lösenord")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
    
}
