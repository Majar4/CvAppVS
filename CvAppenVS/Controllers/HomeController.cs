using System.Diagnostics;
using CvAppenVS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
            var model = new HomeIndexViewModel();
            try
            {
                model.FeaturedCvs = _context.Users
                    .Where(u => u.IsPrivate == false)
                    .Select(u => new CvSummaryViewModel
                    {
                        UserId = u.Id,
                        Name = u.Name,
                        Presentation = u.CV != null ? u.CV.Presentation : "Ingen presentation tillgänglig"
                    })
                    .Take(4)
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
