using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages.Bugs
{
    /// <summary>
    /// Сторінка детального перегляду бага.
    /// Доступна лише авторизованим користувачам з правом перегляду.
    /// </summary>
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IBugService _bugService;
        private readonly IBugAccessService _bugAccessService;

        public DetailsModel(IBugService bugService, IBugAccessService bugAccessService)
        {
            _bugService = bugService;
            _bugAccessService = bugAccessService;
        }

        public Bug Bug { get; set; } = default!;

        /// <summary>Чи може поточний користувач видалити цей баг.</summary>
        public bool CanDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);

            if (bug == null)
                return NotFound();

            if (!await _bugAccessService.CanAccessAsync(User, bug))
                return Forbid();

            Bug = bug;
            CanDelete = await _bugAccessService.CanDeleteAsync(User, bug);
            return Page();
        }
    }
}