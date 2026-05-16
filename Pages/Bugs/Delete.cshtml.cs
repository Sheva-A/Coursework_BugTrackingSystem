using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages.Bugs
{
    /// <summary>
    /// Сторінка підтвердження видалення бага.
    /// Доступна лише адміністратору або автору бага.
    /// </summary>
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly IBugService _bugService;
        private readonly IBugAccessService _bugAccessService;

        public DeleteModel(IBugService bugService, IBugAccessService bugAccessService)
        {
            _bugService = bugService;
            _bugAccessService = bugAccessService;
        }

        public Bug Bug { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);
            if (bug == null) return NotFound();

            if (!await _bugAccessService.CanDeleteAsync(User, bug))
                return Forbid();

            Bug = bug;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);
            if (bug == null) return NotFound();

            if (!await _bugAccessService.CanDeleteAsync(User, bug))
                return Forbid();

            await _bugService.DeleteBugAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
