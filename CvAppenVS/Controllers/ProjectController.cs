using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAppenVS.Models;

namespace CvAppenVS.Controllers
{
    public class ProjectController : Controller
    {
        private readonly CvContext _context;

        public ProjectController(CvContext context)
        {
            _context = context; 
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
                Date = DateTime.Now
            };

            //sparar
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            //skikcas tillbaks till projektlistan
            return RedirectToAction(nameof(Index));

        }
    }
}
