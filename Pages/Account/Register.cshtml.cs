using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BugTrackingSystem.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTrackingSystem.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<SelectListItem> Roles { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email є обов'язковим")]
            [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль є обов'язковим")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має містити від 6 до 100 символів")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Підтвердіть пароль")]
            [Compare("Password", ErrorMessage = "Паролі не збігаються")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Оберіть роль")]
            public string SelectedRole { get; set; }
        }

        public void OnGet()
        {
            PopulateRoles();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Input.SelectedRole);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            PopulateRoles();

            return Page();
        }

        private void PopulateRoles()
        {
            Roles = _roleManager.Roles
                .Where(r => r.Name != AppRoles.Admin)
                .Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                .ToList();
        }
    }
}