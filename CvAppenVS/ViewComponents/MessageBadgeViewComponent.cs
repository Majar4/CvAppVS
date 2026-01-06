using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CvAppen.Data;

namespace CvAppen.Web.ViewComponents
{

    
    public class MessageBadgeViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly CvContext _context;

        public MessageBadgeViewComponent(UserManager<User> userManager, CvContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(0);
            }

            var userId = _userManager.GetUserId(HttpContext.User);
            var unreadMessageCount = await _context.Messages
                .Where(m => m.ToUserId == userId && !m.IsRead)
                .CountAsync();
            return View(unreadMessageCount);
        }
    }
}
