using CvAppen.Web.DTOs;
using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CvAppen.Web.Controllers
{
    /// Hanterar meddelanden mellan användare:
    /// visa inkorg, skicka meddelanden, ta bort, markera som läst och visa skickade meddelanden.

    public class MessageController : Controller
    {
        private readonly CvContext _context;
        private readonly UserManager<User> _userManager;

        public MessageController(CvContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// Visar inloggad användares inkorg.
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
                    FromUserId = m.FromUserId,
                    IsRead = m.IsRead,
                    //HasReplied = m.HasReplied, måste läggas till i databasen i så fall.
                    SentAt = m.SentAt
                }).OrderByDescending(m => m.SentAt)
                .ToListAsync<MessageDto>();

            ViewBag.Message = "Inkorg";

            return View(message);
        }

        /// Visar formulär för att skicka ett nytt meddelande.
        [HttpGet]
        public async Task<IActionResult> Add(string toUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            ViewBag.Users = await _context.Users
                    .Where(u => u.Id != currentUserId)
                    .Select(u => new {
                        Value = u.Id,
                        Text = u.Name
                    })
                    .ToListAsync();

            if (!string.IsNullOrEmpty(toUserId))
            {
                var recipient = await _userManager.FindByIdAsync(toUserId);
                ViewBag.RecipientName = recipient?.Name;
            }

            return View(new SendMessageDto
            {
                ToUserId = toUserId
            });
        }

        /// Skickar ett nytt meddelande till vald mottagare.
        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> SendMessage(SendMessageDto dto, int? replyingToId)
        {
            if (!User.Identity.IsAuthenticated && string.IsNullOrWhiteSpace(dto.SenderName))
            {
                ModelState.AddModelError(nameof(dto.SenderName), "Du måste ange ett namn.");
            }

            if (!ModelState.IsValid) return View("Add", dto);

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

            // Förhindrar att användare skickar meddelande till sig själv
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
                if (replyingToId.HasValue)
                {
                    var originalMessage = await _context.Messages.FindAsync(replyingToId.Value);
                    if (originalMessage != null)
                    {
                        originalMessage.IsRead = true;
                        //originalMessage.HasReplied = true; // för att få symbol om svarat måste tabell läggas till i databasen.
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Meddelandet kunde inte skickas.";
                return View(dto);
            }
            TempData["ToastMessage"] = "Meddelandet har skickats!";
            return RedirectToAction(nameof(Index));
        }


        /// Visar bekräftelsesida för att ta bort meddelande.
        [HttpGet]
        [Authorize]
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


        /// Tar bort meddelande permanent.
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

        /// Markerar valt meddelande som läst.
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

        /// Visar alla meddelanden som den inloggade användaren har skickat.
        [Authorize]
        public async Task<IActionResult> Sent()
        {
            var currentUserId = _userManager.GetUserId(User);

            var sentMessages = await _context.Messages
                .Where(m => m.FromUserId == currentUserId)
                .OrderByDescending(m => m.SentAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    SenderName = _context.Users.Where(u => u.Id == m.ToUserId).Select(u => u.Name).FirstOrDefault() ?? "Okänd mottagare",
                    IsRead = m.IsRead
                })
                .ToListAsync();

            ViewBag.Message = "Skickade meddelanden";
            return View(sentMessages);
        }

    }

}
