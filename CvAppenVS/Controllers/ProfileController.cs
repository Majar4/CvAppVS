using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static CvAppen.Web.ViewModels.CvViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CvAppen.Web.Controllers
{
    /// Hanterar användarprofiler:
    /// visa profil, visa CV, lista projekt och söka användare.
    
    public class ProfileController : Controller
    {
        public readonly CvContext _context;
        public readonly UserManager<User> _userManager;
        public ProfileController(CvContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        /// Visar profilsidan för inloggad användare eller vald användare.
        public async Task<IActionResult> Index(string? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            // Vilken användares profil som visas
            User profileUser;
            if (string.IsNullOrEmpty(id))
            {
                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                profileUser = currentUser;  
            }
            else
            {
                profileUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (profileUser == null)
                {
                    return NotFound();
                }
            }

            // Hämta CV för vald användare
            var cvEntity = await _context.CVs
                .Include(c => c.Competences)
                .Include(c => c.Educations)
                .Include(c => c.EarlierExperiences)
                .FirstOrDefaultAsync(c => c.UserId == profileUser.Id);


            // Skapar ett ViewModel-objekt för visning i vyn
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

            // Hämta projekt som användaren deltar i
            var projects = _context.UserProjects
                .Where(up => up.UserId == profileUser.Id)
                .Select(up => new ProfileProjectsViewModel
                {
                    Id = up.Project.Id,
                    Title = up.Project.Title
                })
                .ToList();

            // Bygg hela profilsidans ViewModel
            var model = new MyProfileViewModel
            {
                UserId = profileUser.Id,
                Name = profileUser.Name,
                MyProjects = projects,
                CV = cvViewModel,
                ProfilePictureUrl = string.IsNullOrEmpty(profileUser.Image)
                        ? "default-profile.png"
                        : profileUser.Image,
                CanEditCv = currentUserId != null && currentUserId == profileUser.Id,   
                VisitCount = profileUser.VisitCount,
                IsOwner = true,
                
                UnReadMessagesCount = currentUserId != null
                    ? _context.Messages.Count(m => m.ToUserId == currentUserId && !m.IsRead)
                    : 0
            };

            return View(model);
        }

        /// Söker efter användare baserat på namn eller e-post.
        /// Privata profiler visas endast för inloggade användare.
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Search(string searchName, string searchSkill)
        {
            var query = _context.Users
            .Include(u => u.CV)
            .ThenInclude(cv => cv.Competences)
            .Where(u => u.IsActive)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                query = query.Where(u => u.Name.Contains(searchName) || u.Email.Contains(searchName));
            }

            if (!string.IsNullOrWhiteSpace(searchSkill))
            {
                query = query.Where(u => u.CV != null &&
                                         u.CV.Competences.Any(c => c.Title.Contains(searchSkill)));
            }

            if (!User.Identity.IsAuthenticated)
            {
                query = query.Where(u => u.IsPrivate == false);
            }

            var result = await query.ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.SearchSkill = searchSkill;

            return View(result);
        }

        /// Visar sida för en annan användares profil/CV.
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return RedirectToAction("Index");

            var currentUserId = _userManager.GetUserId(User);

            if (id == currentUserId)
            {
                return RedirectToAction("Index");
            }
            if (string.IsNullOrEmpty(id)) return NotFound();

            var profileUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (profileUser == null) return NotFound();

            if (currentUserId == null || profileUser.Id != currentUserId)
            {
                profileUser.VisitCount++;
                await _context.SaveChangesAsync();
            }

            // Hämta CV
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
                    UserId = cvEntity.UserId,
                    Competences = cvEntity.Competences.Select(c => new CompetenceViewModel { Id = c.Id, Title = c.Title }).ToList(),
                    Educations = cvEntity.Educations.Select(e => new EducationViewModel { Id = e.Id, Name = e.Name, School = e.School, From = e.From, To = e.To }).ToList(),
                    EarlierExperiences = cvEntity.EarlierExperiences.Select(ex => new EarlierExperienceViewModel { Id = ex.Id, Title = ex.Title, Company = ex.Company, Description = ex.Description, From = ex.From, To = ex.To }).ToList()
                };
            }

            //Hämta användarens projekt
            var projects = await _context.UserProjects
                .Where(up => up.UserId == profileUser.Id)
                .Select(up => new ProfileProjectsViewModel
                {
                    Id = up.Project.Id,
                    Title = up.Project.Title
                })
                .ToListAsync();

            var currentSkills = new List<string> ();
            if(cvEntity != null)
            {
                currentSkills = cvEntity.Competences.Select(c => c.Title.Trim()).ToList();
            }

            List<User> similarUsers = new List<User>();

            bool isAuthenticated = User.Identity.IsAuthenticated;

            if (currentSkills.Any())
            {
                similarUsers = await _context.Users
                        .Include(u => u.CV)
                        .ThenInclude(cv => cv.Competences)
                        .Where(u => u.Id != profileUser.Id
                                    && u.IsActive
                                    && u.CV != null
                                    && u.CV.Competences.Any(c => currentSkills.Contains(c.Title)))
                        .Where(u => isAuthenticated || !u.IsPrivate)
                        .OrderByDescending(u => u.CV.Competences.Count(c => currentSkills.Contains(c.Title)))
                        .Take(3)
                        .ToListAsync();
            }

            var model = new MyProfileViewModel
            {
                UserId = profileUser.Id,
                Name = profileUser.Name,
                ProfilePictureUrl = profileUser.Image,
                CV = cvViewModel,
                MyProjects = projects,
                CanEditCv = false,
                SimilarProfiles = similarUsers
            };

            return View(model);
        }
    }
}
