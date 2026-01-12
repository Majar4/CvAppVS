using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CvAppenVS.Controllers
{
    /// Hanterar användarkonton:
    /// inloggning, utloggning, registrering och kontoinställningar.
    
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

        /// Visar inloggningssidan.
        [HttpGet]
        public IActionResult LogIn()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        /// Tar emot inloggningsuppgifter och autentiserar användaren.
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
                ModelState.AddModelError(string.Empty, "Fel användarnamn eller lösenord");
                return View(vm);
            }
                
            return RedirectToAction("Index", "Home");
        }

        /// Loggar ut den inloggade användaren.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        /// Visar registreringsformulär.
        [HttpGet]
        public IActionResult Registrera()
        {
            return View();
        }

        /// Visar inställningar för inloggad användare.
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

        /// Sparar ändringar i användarens kontoinställningar.
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

            try
            {
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(vm);
                }
                TempData["Success"] = "Inställningarna har uppdaterats";
                return RedirectToAction("Settings");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Det gick inte att spara ändringarna.";
                return View(vm);
            }
        }

        /// Visar formulär för byte av lösenord.
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(); 
        }

        /// Tar emot och genomför lösenordsbyte.
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await userManager.GetUserAsync(User);


            try 
            {
                var result = await userManager.ChangePasswordAsync(
                    user,
                    vm.CurrentPassword,
                    vm.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)

                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError(string.Empty, "Nuvarande lösenord är felaktigt.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                    return View(vm);
                }
                await signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Lösenordet har ändrats";
                return RedirectToAction("Settings");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Lösenordet kunde inte ändras.";
                return View(vm);
            }
        }

        /// Tar emot registreringsuppgifter och skapar nytt konto.
        [HttpPost]
        public async Task<IActionResult> Registrera(RegistreraViewmodel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            string fileName = "default-profile.png";

            if (request.Image != null && request.Image.Length > 0)
            {

                fileName = Guid.NewGuid() + Path.GetExtension(request.Image.FileName);
                string imagePath = Path.Combine(
                environment.WebRootPath,
                "images",
                fileName
                );

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.UserName,
                Name = request.Name,
                Address = request.Address,
                Image = fileName
            };

            try
            {
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
                
                TempData["Success"] = "Registreringen lyckades! Du kan nu logga in.";
                return RedirectToAction("LogIn");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Anslutning till databasen misslyckades. Försök igen.";
                return View(request);
            }
        }
    }   
}
