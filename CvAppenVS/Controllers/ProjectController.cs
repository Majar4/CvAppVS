using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAppen.Data;
using CvAppenVS.Web.Models;
using Microsoft.AspNetCore.Identity;



namespace CvAppenVS.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvContext _context;
        private readonly UserManager<User> _userManager; //Hämtar inloggad användare

        public ProjectController(CvContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Lista alla projekt
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.UserProjects)
                .ThenInclude(up => up.User)
                .ToListAsync();
            return View(projects);
        }

        // Visar formuläret för att skapa ett nytt projekt
        public IActionResult Create()
        {
            return View();
        }

        // Tar emot formuläret och sparar projektet i databasen
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            // Kontrollerar att formuläret är korrekt ifyllt
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Skapar ett nytt projektobjekt från formulärets data
            var project = new Project
            {
                Title = model.Title,
                Description = model.Description,
                //TODO : lägg till datum
            };

            //sparar
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Koppling mellan användaren/skaparen och projektet
            var user = await _userManager.GetUserAsync(User);

            var link = new UserProject
            {
                UserId = user.Id,
                ProjectId = project.Id,
            };

            _context.UserProjects.Add(link);
            await _context.SaveChangesAsync();

            //skikcas tillbaks till projektlistan
            return RedirectToAction(nameof(Index));

        }
    }
}
