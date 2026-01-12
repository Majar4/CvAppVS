using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CvAppen.Web.ViewModels.CvViewModel;

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
                    .Where(c => !string.IsNullOrWhiteSpace(c.Title))
                    .Select(c => new Competence 
                    {
                        Id = c.Id,
                        Title = c.Title
                    })
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
            //return RedirectToAction("Index", "Profile", new { id = cvt.Id });
            return RedirectToAction("Index", "Profile");
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

            var cvt = new CvViewModel
            {
                Id = cv.Id,
                Presentation = cv.Presentation,
                PhoneNumber = cv.PhoneNumber,
                ImagePath = cv.ImagePath,
                UserId = cv.UserId,
                UserName = cv.User?.UserName,

                Competences = cv.Competences
                    .Where(c => !string.IsNullOrWhiteSpace(c.Title))
                    .Select(c => new CvViewModel.CompetenceViewModel
                    {
                        Id = c.Id,
                        Title = c.Title
                    })
                    .ToList(),

                Educations = cv.Educations
                    .Where(e => !string.IsNullOrWhiteSpace(e.Name))
                    .Select(e => new CvViewModel.EducationViewModel
                    {
                        Id = e.Id,
                        Name = e.Name,
                        School = e.School,
                        From = e.From,
                        To = e.To
                    })
                    .ToList(),

                EarlierExperiences = cv.EarlierExperiences
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.Title))
                    .Select(ex => new CvViewModel.EarlierExperienceViewModel
                    {
                        Id = ex.Id,
                        Title = ex.Title,
                        Company = ex.Company,
                        Description = ex.Description,
                        From = ex.From,
                        To = ex.To
                    })
                    .ToList()
            };

            return View(cvt);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CvViewModel model)
        {
           if(!ModelState.IsValid)
            {
                return View(model);
            }
           
                var cv = await _context.CVs
                    .Include(c => c.Competences)
                    .Include(c => c.Educations)
                    .Include(c => c.EarlierExperiences)
                    .FirstOrDefaultAsync(c => c.Id == model.Id);

                if (cv == null)
                {
                    return NotFound();
                }

            cv.Presentation = model.Presentation;
            cv.PhoneNumber = model.PhoneNumber;

                var currentUser = await _userManager.GetUserAsync(User);
                if (cv.UserId != currentUser.Id)
                {
                    return Forbid();
                }

            cv.Competences.Clear();

            foreach (var c in model.Competences.Where(x => !string.IsNullOrWhiteSpace(x.Title)))
            {
                cv.Competences.Add(new Competence
                {
                    Title = c.Title
                });
            }

            cv.Educations.Clear();
            foreach (var e in model.Educations.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
            {
                cv.Educations.Add(new Education
                {
                    Name = e.Name,
                    School = e.School,
                    From = e.From,
                    To = e.To
                });
            }
            cv.EarlierExperiences.Clear();
            foreach (var ex in model.EarlierExperiences.Where(x => !string.IsNullOrWhiteSpace(x.Title)))
            {
                cv.EarlierExperiences.Add(new EarlierExperience
                {
                    Title = ex.Title,
                    Company = ex.Company,
                    Description = ex.Description,
                    From = ex.From,
                    To = ex.To
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            var cvEntity = await _context.CVs
                .Include(c => c.Competences)
                .Include(c => c.Educations)
                .Include(c => c.EarlierExperiences)
                .Include(c => c.User) 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cvEntity == null) return NotFound();

            var model = new CvViewModel
            {
                Id = cvEntity.Id,
                UserName = cvEntity.User?.Name,
                Presentation = cvEntity.Presentation,
                PhoneNumber = cvEntity.PhoneNumber,
                Educations = cvEntity.Educations.Select(e => new EducationViewModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    School = e.School,
                    From = e.From,
                    To = e.To
                }).ToList(),
                EarlierExperiences = cvEntity.EarlierExperiences.Select(ex => new EarlierExperienceViewModel
                {
                    Id = ex.Id,
                    Title = ex.Title,
                    Company = ex.Company,
                    Description = ex.Description,
                    From = ex.From,
                    To = ex.To
                }).ToList(),
                Competences = cvEntity.Competences.Select(c => new CompetenceViewModel
                {
                    Id = c.Id,
                    Title = c.Title
                }).ToList()
            };

            return View("Details", model);
        }

    }
}


