using CvAppen.Data;
using CvAppenVS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;



namespace CvAppenVS.Controllers
{
    /// Ansvarar för funktionalitet kopplad till projekt:
    /// visa, skapa, redigera och gå med i projekt.
    public class ProjectController : Controller
    {
        private readonly CvContext _context;
        private readonly UserManager<User> _userManager; 

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

        // Tar emot formuläret och skapar ett nytt projekt i databasen
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectViewModel model)
        {
        
            if (!ModelState.IsValid)
            {
                return View(model);
            }
        
            var user = await _userManager.GetUserAsync(User);
            
            var project = new Project
            {
                Title = model.Title,
                Description = model.Description,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedByUserId = user.Id
            };

            try
            {
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

            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Projektet kunde inte skapas.";
                return View(model);
            }
            
            return RedirectToAction(nameof(Index));

        }

        //Lägger till den inloggade användaren som deltagare i ett befintligt projekt.
        [Authorize]
        public async Task<IActionResult> Join(int projectId)
        {
            var user = await _userManager.GetUserAsync(User);

            bool alreadyJoined = await _context.UserProjects
                .AnyAsync(up => up.ProjectId == projectId && up.UserId == user.Id);


            if (!alreadyJoined)
            {
                
                try
                {
                    var userProject = new UserProject
                    {
                        ProjectId = projectId,
                        UserId = user.Id
                    };

                    _context.UserProjects.Add(userProject);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    TempData["Error"] = "Anslutning till databasen misslyckades. Du kunde inte gå med i projektet.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        //Visar redigeringsformulär för projekt. Endast projektets skapare har behörighet. 
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            //Endast skaparen får redigera
            if (project.CreatedByUserId != userId)
                return Forbid();

            return View(project);
        }

        //Tar emot ändringar och uppdaterar projektets information i databasen.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(Project editedProject)
        {
            var userId = _userManager.GetUserId(User);
            var project = await _context.Projects.FindAsync(editedProject.Id);

            if (project == null)
                return NotFound();

            if (project.CreatedByUserId != userId)
                return Forbid();

            project.Title = editedProject.Title;
            project.Description = editedProject.Description;
            project.FromDate = editedProject.FromDate;
            project.ToDate = editedProject.ToDate;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Projektet kunde inte uppdateras.";
                return View(project);
            }

            return RedirectToAction(nameof(Index));


        }
    }
}
