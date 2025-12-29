using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvAppen.Data;
using CvAppen.Web.ViewModels;

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
    }
}
