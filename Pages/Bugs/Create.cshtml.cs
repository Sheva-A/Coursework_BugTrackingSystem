using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;
using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BugTrackingSystem.Pages.Bugs
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IBugService _bugService;
        private readonly IBugAccessService _bugAccessService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CreateModel(IBugService bugService, IBugAccessService bugAccessService,
            UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _bugService = bugService;
            _bugAccessService = bugAccessService;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public Bug Bug { get; set; } = default!;

        public SelectList UsersList { get; set; }
        public SelectList ProjectsList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await PopulateLists(null);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Challenge();

            Bug.AuthorId = userId;

            // Навігаційні властивості не прив'язуються з форми
            ModelState.Remove("Bug.Author");
            ModelState.Remove("Bug.AuthorId");
            ModelState.Remove("Bug.AssignedTo");
            ModelState.Remove("Bug.Project");

            // Виконавець повинен бути учасником обраного проєкту
            if (Bug.ProjectId.HasValue && !string.IsNullOrEmpty(Bug.AssignedToId))
            {
                var isMember = await _context.ProjectMembers.AnyAsync(pm =>
                    pm.ProjectId == Bug.ProjectId && pm.UserId == Bug.AssignedToId);
                if (!isMember)
                    ModelState.AddModelError("Bug.AssignedToId",
                        "Обраний виконавець не є учасником цього проєкту.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateLists(Bug.ProjectId);
                return Page();
            }

            await _bugService.CreateBugAsync(Bug);
            return RedirectToPage("./Index");
        }

        // Заповнює списки виконавців та проєктів з урахуванням обраного проєкту
        private async Task PopulateLists(int? projectId)
        {
            if (projectId.HasValue)
            {
                // Показуємо лише учасників обраного проєкту
                var memberIds = await _context.ProjectMembers
                    .Where(pm => pm.ProjectId == projectId.Value)
                    .Select(pm => pm.UserId)
                    .ToListAsync();
                var members = await _userManager.Users
                    .Where(u => memberIds.Contains(u.Id))
                    .OrderBy(u => u.UserName)
                    .ToListAsync();
                UsersList = new SelectList(members, "Id", "UserName");
            }
            else
            {
                // Без проєкту — усі користувачі
                var all = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
                UsersList = new SelectList(all, "Id", "UserName");
            }

            var projectsQuery = _context.Projects.AsQueryable();
            projectsQuery = await _bugAccessService.ApplyProjectFilterAsync(User, projectsQuery);
            var projects = await projectsQuery.OrderBy(p => p.Name).ToListAsync();
            ProjectsList = new SelectList(projects, "Id", "Name");
        }
    }
}