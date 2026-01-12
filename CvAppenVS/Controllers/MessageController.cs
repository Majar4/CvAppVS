using CvAppen.Web.DTOs;
using CvAppen.Data;
using CvAppen.Web.ViewModels;
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
        [Authorize]
        [ActionName("Index")]
        public async Task<IActionResult> ReadMessages()
        {
            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages
                .Where(m => m.ToUserId == userId) 
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Text,
                    SenderName = m.SenderName,
                    IsRead = m.IsRead,
                    SentAt = m.SentAt
                }).OrderBy(m => m.SentAt)
                .ToListAsync<MessageDto>();

            ViewBag.Message = "Inkorg";

            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Add(string toUserId)
        {
            var dto = new SendMessageDto
            {
                ToUserId = toUserId
            };

            return View(dto);
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


            if (dto.ToUserId == userId)
            {
                return BadRequest();
            }


            var message = new Message
            {


                Text = dto.Text,
                FromUserId = userId,
                ToUserId = dto.ToUserId,
                SenderName = senderName,
                IsRead = false,
                SentAt = DateTime.Now
            };

            try
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Meddelandet kunde inte skickas.";
                return View(dto);
            }

            return RedirectToAction("Index", "Profile", new { id = dto.ToUserId });
        }


        [HttpGet]
        public async Task<IActionResult> SendTestMessage()
        {

            
            var receiver = await _userManager.FindByEmailAsync("Tomten@hotmail.com");
            var message = new Message
            {
                Text = "En kaffe?",
                FromUserId = null,
                ToUserId = receiver.Id,
                SenderName = "Gillis",
                IsRead = false,
                SentAt = DateTime.Now
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok("Testmeddelande skickat");
        }

        [HttpGet]
        public async Task <IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages

               .FirstOrDefaultAsync(m => m.Id == id);
            if(message == null)
            {
                return NotFound();
            }

            if (message.ToUserId != userId)
            {
                return Forbid();
            } 

            return View(message);

        }


        [HttpPost]
        [Authorize]
        public async Task <IActionResult> RemoveConfirmed(int id)
        {

            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            if (message.ToUserId != userId)
            {
                return Forbid();
            }

            try
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["Error"] = "Anslutning till databasen misslyckades. Meddelandet kunde inte tas bort.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task <IActionResult> MarkAsRead (int id)
        {
            var userId = _userManager.GetUserId(User);

            var message = await _context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            if (message.ToUserId != userId)
            {
                return Forbid();
            }

            try
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                TempData["Error"] = "Anslutning till databasen misslyckades. Meddelandet kunde inte markeras som läst.";
            }

            return RedirectToAction(nameof(Index));
        }

    }

}
