using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CvAppen.Data;
using CvAppen.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CvAppenVS.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]

        public IActionResult LogIn()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
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
    }
    
       
}
