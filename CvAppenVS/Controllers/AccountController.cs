using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CvAppenVS.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IWebHostEnvironment environment;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await signInManager.PasswordSignInAsync(
                vm.UserName,
                vm.Password,
                vm.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Gick ej att logga in");
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registrera()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Settings()
        {
            var user = await userManager.GetUserAsync(User);
            var vm = new AccountSettingsViewModel
            {
                Name = user.Name,
                Address = user.Address,
                IsPrivate = user.IsPrivate
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Settings(AccountSettingsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await userManager.GetUserAsync(User);
            user.Name = vm.Name;
            user.Address = vm.Address;
            user.IsPrivate = vm.IsPrivate;

            await userManager.UpdateAsync(user);

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Registrera(RegistreraViewmodel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);

            string imagePath = Path.Combine(
                environment.WebRootPath,
                "images",
                fileName
                );

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }
            var user = new User
            {
                UserName = request.UserName,
                Email = request.UserName,
                Name = request.Name,
                Address = request.Address,
                Image = fileName
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    Console.WriteLine(error.Description);
                }
                return View(request);
            }

            

            TempData["SuccessMessage"] = "Registreringen lyckades! Du kan nu logga in.";

            return RedirectToAction("LogIn");

        }
        


    }   
}
