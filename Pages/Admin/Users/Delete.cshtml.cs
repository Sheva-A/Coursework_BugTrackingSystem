using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using System.Security.Claims;

namespace BugTrackingSystem.Pages.Admin.Users
{
    /// <summary>
    /// Сторінка підтвердження видалення користувача (лише для адміністратора).
    /// FK Bug→Author налаштовано як Restrict, тому баги автора видаляються вручну перед видаленням користувача.
    /// Членство в проєктах та призначення на багах видаляються каскадно/SetNull.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DeleteModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public ApplicationUser TargetUser { get; set; } = null!;
        public List<string> TargetRoles   { get; set; } = new();
        public int AuthoredBugsCount      { get; set; }
        public int ProjectCount           { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Не можна видалити себе або іншого адміна
            if (user.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return Forbid();

            TargetUser        = user;
            TargetRoles       = (await _userManager.GetRolesAsync(user)).ToList();
            AuthoredBugsCount = await _context.Bugs.CountAsync(b => b.AuthorId == id);
            ProjectCount      = await _context.ProjectMembers.CountAsync(pm => pm.UserId == id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Повторна перевірка безпеки
            if (user.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)) return Forbid();
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return Forbid();

            // Restrict FK не дозволяє видалення автора з активними багами — видаляємо вручну
            var authoredBugs = await _context.Bugs.Where(b => b.AuthorId == id).ToListAsync();
            _context.Bugs.RemoveRange(authoredBugs);
            await _context.SaveChangesAsync();

            // Після видалення багів: ProjectMembers → Cascade, AssignedBugs → SetNull
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty,
                    "Помилка видалення: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                TargetUser        = user;
                TargetRoles       = (await _userManager.GetRolesAsync(user)).ToList();
                AuthoredBugsCount = authoredBugs.Count;
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
