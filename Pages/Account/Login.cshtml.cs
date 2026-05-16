using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BugTrackingSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email є обов'язковим")]
            [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль є обов'язковим")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запам'ятати мене?")]
            public bool RememberMe { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email, 
                    Input.Password, 
                    Input.RememberMe, 
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                
                ModelState.AddModelError(string.Empty, "Невірний логін або пароль.");
            }

            return Page();
        }
    }
}