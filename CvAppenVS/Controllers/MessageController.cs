using CvAppenVS.Models;
using CvAppenVS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CvAppenVS.Controllers
{

    public class MessageController : Controller
    {
        private readonly CvContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(CvContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        //[Authorize]
        [ActionName("Index")]
        public async Task<IActionResult> ReadMessages()
        {
            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages
                //.Where(m => m.ToUserId == userId) <-- LÄGG TILL NÄR INLOGG FINNS
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Text,
                    SenderName = m.SenderName,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt
                }).OrderBy(m => m.SentAt)
                .ToListAsync<MessageDto>();

            ViewBag.Message = "Mina meddelanden:";

            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            string senderName;
            string userId = null;

            if (User.Identity.IsAuthenticated)
            {
                userId = _userManager.GetUserId(User);
                var user = await _userManager.FindByIdAsync(userId);
                senderName = user.Name;
            }
            else
            {
                senderName = dto.SenderName;
            }
            var message = new Message
            {
                Text = dto.Text,
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                SenderName = dto.SenderName,
                IsRead = false,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok();
        }




    }

}

