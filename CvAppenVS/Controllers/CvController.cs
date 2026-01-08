using CvAppen.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CvAppen.Web.Controllers
{
    public class CvController : Controller
    {
        private readonly CvContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CvController(CvContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cv = await _context.CVs
                .Include(c => c.Competences)
                .Include(c => c.Educations)
                .Include(c => c.EarlierExperiences)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

            return View(cv);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CV cv, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = _userManager.GetUserId(User);
                cv.UserId = userId;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cv");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    cv.ImagePath = "/images/cv/" + uniqueFileName;
                }
                else
                {
                 
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        cv.ImagePath = "/images/" + user.Image;
                    }
                }

                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = cv.Id });
            }
            return View(cv);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cv = await _context.CVs
        .Include(c => c.Competences)
        .Include(c => c.Educations)
        .Include(c => c.EarlierExperiences)
        .Include(c => c.User)
        .FirstOrDefaultAsync(c => c.Id == id);

            if (cv == null)
            {
                return NotFound();
            }

          var currentUser = await _userManager.GetUserAsync(User);
            if(cv.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(cv);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CV updatedCv)
        {
            if (id != updatedCv.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var cv = await _context.CVs
                    .Include(c => c.Competences)
                    .Include(c => c.Educations)
                    .Include(c => c.EarlierExperiences)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (cv == null)
                {
                    return NotFound();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                if (cv.UserId != currentUser.Id)
                {
                    return Forbid();
                }

                cv.Presentation = updatedCv.Presentation;
                cv.PhoneNumber = updatedCv.PhoneNumber;
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = cv.Id });
            }

            return View(updatedCv);
        }
    }
}
