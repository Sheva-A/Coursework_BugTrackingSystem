using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Pages.Account
{
    /// <summary>
    /// Вихід з системи. Підтримує і GET, і POST, щоб покривати вихід з будь-якого місця.
    /// </summary>
    [IgnoreAntiforgeryToken]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }

        // Підтримуємо GET для випадків виходу за прямим посиланням
        public Task<IActionResult> OnGetAsync() => OnPostAsync();
    }
}