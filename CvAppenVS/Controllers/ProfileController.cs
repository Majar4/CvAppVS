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
    }
}
