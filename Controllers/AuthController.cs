using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Notes.Identity.Models;

namespace Notes.Identity.Controllers
{
   
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        private readonly IIdentityServerInteractionService _interactionService;


        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var viewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(loginViewModel);
            }
            var user = await _userManager.FindByNameAsync(loginViewModel.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(loginViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username,
                loginViewModel.Password, false, false);
            if(result.Succeeded)
            {
                return Redirect(loginViewModel.ReturnUrl);  
            }
            ModelState.AddModelError(string.Empty, "Login error");
            return View(loginViewModel);
        }
    }
}
