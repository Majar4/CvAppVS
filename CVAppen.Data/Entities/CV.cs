using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CvAppen.Data
{
    public class CV 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Presentation { get; set; }
        public string PhoneNumber { get; set; }
       
        public string UserId {  get; set; }
        public User User { get; set; }

        public string? ImagePath { get; set; } //sökväg till bilden vi senare ska kunna lägga till

        public ICollection<Competence> Competences { get; set; } = new List<Competence>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<EarlierExperience> EarlierExperiences { get; set; } = new List<EarlierExperience>();
    }
}
