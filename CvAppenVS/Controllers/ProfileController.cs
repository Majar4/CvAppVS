using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CvAppen.Web.ViewModels;
using CvAppen.Data;

namespace CvAppen.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        public readonly CvContext _context;
        public ProfileController(CvContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            var model = new MyProfileViewModel
            {
                Name = user.Name,
       
                UnReadMessagesCount = _context.Messages.Count(m => m.ToUserId == userId && !m.IsRead)
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
        [HttpGet]
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new MyProfileViewModel
            {
                Name = user.Name,
            };

            var projects = _context.UserProjects
            .Where(up => up.UserId == id)
            .Select(up => up.Project)
            .ToList();
            ViewBag.Projects = projects;

            return View(model);
        }
    }
}
