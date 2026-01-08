using CvAppen.Data;
using CvAppen.Web.ViewModels;
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var cvViewModel = new CvViewModel();
            return View(cvViewModel);
        }

        [HttpPost]

        public async Task<IActionResult> Create(CvViewModel cv, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cv);
            }

            var user = await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);
            cv.UserId = userId;
            cv.UserName = user.UserName;


            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cv");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid() + "_" + imageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fs = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(fs);

                cv.ImagePath = "/images/cv/" + uniqueFileName;
            }
            else if (!string.IsNullOrEmpty(user.Image))
            {
                    cv.ImagePath = "/images/" + user.Image;
            }

            var cvt = new CV
            {
               
                Presentation = cv.Presentation,
                PhoneNumber = cv.PhoneNumber,
                UserId = cv.UserId,

                Competences = cv.Competences
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => new Competence { Title = c })
                    .ToList(),

                Educations = cv.Educations
                    .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                    .Select(e => new Education
                    {
                        Name = e.Name,
                        School = e.School,
                        From = e.From,
                        To = e.To
                    })
                    .ToList(),

                EarlierExperiences = cv.EarlierExperiences
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.Title))
                    .Select(ex => new EarlierExperience
                    {
                        Title = ex.Title,
                        Company = ex.Company,
                        Description = ex.Description,
                        From = ex.From,
                        To = ex.To

                    }).ToList()
            };

            _context.CVs.Add(cvt);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile", new { id = cvt.Id });
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
            if (cv.UserId != currentUser.Id)
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
