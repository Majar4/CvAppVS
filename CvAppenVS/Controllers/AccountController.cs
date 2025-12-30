using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CvAppen.Data;
using CvAppen.Web.ViewModels;

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


        [HttpPost]
        public async Task<IActionResult> RegistreraSubmit(RegistreraViewmodel request)
        {
            if(!ModelState.IsValid)
            {
                return View(request);
            }

            //var userExists = await userManager.FindByIdAsync(request.UserName);
            var user = new User
            {
                UserName = request.UserName,
                Email = request.UserName,
                Name = "Fredrik",
                Address = "Orebro",
                Image = "fredrik.jpg"
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(request);
            }

            return View(request);
        }
    }   
}
