using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CvAppen.Web.ViewModels
{
    public class CvViewModel
    {
        public int Id { get; set; }
        
        public string Presentation { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Telefonumret är inte i giltigt format.")]
        public string PhoneNumber { get; set; }
        public string? ImagePath { get; set; }
        [HiddenInput]
        public string? UserName { get; set; }
        [HiddenInput]
        public string? UserId { get; set; }

        public List<CompetenceViewModel> Competences { get; set; } = new List<CompetenceViewModel>();
        public List<EducationViewModel> Educations { get; set; } = new List<EducationViewModel>();
        public List<EarlierExperienceViewModel> EarlierExperiences { get; set; } = new List<EarlierExperienceViewModel>();
       
        public class CompetenceViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }

        public class EducationViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string School { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
        public class EarlierExperienceViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }

            public string Company { get; set; }
            public string? Description { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
        }
    }
}
