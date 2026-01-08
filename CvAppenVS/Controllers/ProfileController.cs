using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static CvAppen.Web.ViewModels.CvViewModel;

namespace CvAppen.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public readonly CvContext _context;
        public readonly UserManager<User> _userManager;
        public ProfileController(CvContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

      
            User profileUser;
            if (string.IsNullOrEmpty(id))
            {
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account"); 
                }
                profileUser = currentUser; // 
            }
            else
            {
                profileUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (profileUser == null)
                {
                    return NotFound();
                }
            }

            var cvEntity = await _context.CVs
                .Include(c => c.Competences)
                .Include(c => c.Educations)
                .Include(c => c.EarlierExperiences)
                .FirstOrDefaultAsync(c => c.UserId == profileUser.Id);

            CvViewModel? cvViewModel = null;

            if (cvEntity != null)
            {
                cvViewModel = new CvViewModel
                {
                    Id = cvEntity.Id,
                    Presentation = cvEntity.Presentation,
                    PhoneNumber = cvEntity.PhoneNumber,
                    ImagePath = cvEntity.ImagePath,
                    UserId = cvEntity.UserId,
                    UserName = profileUser.UserName,
                    Competences = cvEntity.Competences.Select(c => new CompetenceViewModel
                    {
                        Id = c.Id,
                        Title = c.Title
                    }).ToList(),
                    Educations = cvEntity.Educations.Select(e => new EducationViewModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        School = e.School,
                        From = e.From,
                        To = e.To
                    }).ToList(),
                    EarlierExperiences = cvEntity.EarlierExperiences.Select(ex => new EarlierExperienceViewModel
                    {
                        Id = ex.Id,
                        Title = ex.Title,
                        Company = ex.Company,
                        Description = ex.Description,
                        From = ex.From,
                        To = ex.To
                    }).ToList()
                };
            }

            var projects = _context.UserProjects
                .Where(up => up.UserId == profileUser.Id)
                .Select(up => new ProfileProjectsViewModel
                {
                    Id = up.Project.Id,
                    Title = up.Project.Title
                })
                .ToList();

            // Bygg ViewModel
            var model = new MyProfileViewModel
            {
                UserId = profileUser.Id,
                Name = profileUser.Name,
                MyProjects = projects,
                CV = cvViewModel,
                CanEditCv = currentUserId != null && currentUserId == profileUser.Id,   
                UnReadMessagesCount = currentUserId != null
                    ? _context.Messages.Count(m => m.ToUserId == currentUserId && !m.IsRead)
                    : 0
            };

            return View(model);
        }


        [HttpGet]
        public IActionResult Search(string searchString)
        {
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.Contains(searchString) || u.Email.Contains(searchString));
            }
            return View(users);
        }
        //[HttpGet]
        //public IActionResult Details(string id)
        //{
        //    if (string.IsNullOrEmpty(id))
        //    {
        //        return NotFound();
        //    }

        //    var user = _context.Users.FirstOrDefault(u => u.Id == id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new MyProfileViewModel
        //    {
        //        Name = user.Name,
        //    };

        //    var projects = _context.UserProjects
        //    .Where(up => up.UserId == id)
        //    .Select(up => up.Project)
        //    .ToList();
        //    ViewBag.Projects = projects;

        //    return View(model);
        //}
    }
}
