using CvAppen.Data;
using CvAppenVS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
            // Kontrollerar att formuläret är korrekt ifyllt
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Hämta inloggad användare
            var user = await _userManager.GetUserAsync(User);
            // Skapar ett nytt projektobjekt från formulärets data
            var project = new Project
            {
                Title = model.Title,
                Description = model.Description,
                CreatedByUserId = user.Id
                //TODO : lägg till datum
            };

            //sparar
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Koppling mellan användaren/skaparen och projektet
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

        //Gå med i projekt
        [Authorize]
        public async Task<IActionResult> JoinProject(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);

            bool alreadyJoined = await _context.UserProjects
                .AnyAsync(up => up.ProjectId == projectId && up.UserId == user.Id);


            if (!alreadyJoined)
            {
                // Skapar kopplingen mellan användare och projekt
                var userProject = new UserProject
                {
                    ProjectId = projectId,
                    UserId = user.Id
                };

                _context.UserProjects.Add(userProject);
                await _context.SaveChangesAsync();  
            }

            return RedirectToAction(nameof(Index));
        }

        //Redigera projekt om man har skapat projektet
       
    }
}
