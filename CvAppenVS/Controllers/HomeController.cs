using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CvAppen.Data;
using CvAppen.Web.ViewModels;

namespace CvAppenVS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CvContext _context;

        public HomeController(ILogger<HomeController> logger, CvContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {

            string? currentUserId = null;

            if(User.Identity != null && User.Identity.IsAuthenticated)
            {
                currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            }

            var model = new HomeIndexViewModel();
            try
            {
                model.FeaturedCvs = _context.Users
                    .Where(u => u.IsPrivate == false)
                    .Where(u => currentUserId == null|| u.Id != currentUserId)
                    .Select(u => new CvSummaryViewModel
                    {
                        UserId = u.Id,
                        Name = u.Name,
                        Presentation = u.CV != null ? u.CV.Presentation : "Ingen presentation tillgänglig"
                        //Om vi ska ha in varje användares senste projekt på startsidan, fixa till och avkommentera nedan
                        //LatestProjectTitle = u.UserProjects != null && u.UserProjects.Any()
                        //? u.UserProjects.OrderByDescending(up => up.ProjectId).First().Project.Title
                        //: "Inget projekt än"
                    })
                    .Take(6)
                    .ToList();
                var lastProject = _context.Projects
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault();
                if (lastProject != null)
                {
                    model.LatestProject = new ProjectSummaryViewModel
                    {
                        Title = lastProject.Title,
                        Description = lastProject.Description
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel uppstod vid hämtning av startsidans data.");
                ViewBag.ErrorMessage = "Just nu kan vi inte ladda all information på startsidan. Försök igen senare.";
            }
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
