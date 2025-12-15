using IaEapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IaEapp.Controllers {
    [Authorize]
    public class AccountController : Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult AccessDenied() {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl) {
            LoginViewModel login = new LoginViewModel();
            login.ReturnUrl = returnUrl;
            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel login) {
            if (ModelState.IsValid) {
                AppUser user = await _userManager.FindByEmailAsync(login.Email);
                if (user != null) {
                    await _signInManager.SignOutAsync();
                    SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, login.Remember, false);
                    if (result.Succeeded)
                        return Redirect(login.ReturnUrl ?? "/");
                }
                ModelState.AddModelError(nameof(login.Email), "Nepodařilo se přihlásit. Chybný email nebo heslo");
            }
            return View(login);
        }
        
        public async Task<IActionResult> LogoutAsync() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
