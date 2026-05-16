using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;
using Microsoft.AspNetCore.Identity;
using BugTrackingSystem.Data;

namespace BugTrackingSystem.Pages.Bugs
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IBugService _bugService;
        private readonly IBugAccessService _bugAccessService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EditModel(
            IBugService bugService,
            IBugAccessService bugAccessService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var bug = await _bugService.GetBugByIdAsync(id);
            if (bug == null) return NotFound();

            if (!await _bugAccessService.CanAccessAsync(User, bug)) return Forbid();

            Bug = bug;
            await PopulateLists(bug.ProjectId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var existingBug = await _bugService.GetBugByIdAsync(id);
            if (existingBug == null) return NotFound();
            if (!await _bugAccessService.CanAccessAsync(User, existingBug)) return Forbid();

            ModelState.Remove("Bug.Author");
            ModelState.Remove("Bug.AuthorId");
            ModelState.Remove("Bug.AssignedTo");
            ModelState.Remove("Bug.Project");

            // Validate: assignee must be a member of the selected project
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
                Bug = existingBug;
                await PopulateLists(Bug.ProjectId);
                return Page();
            }

            existingBug.Title = Bug.Title;
            existingBug.Description = Bug.Description;
            existingBug.Status = Bug.Status;
            existingBug.Priority = Bug.Priority;
            existingBug.ProjectId = Bug.ProjectId;
            existingBug.AssignedToId = Bug.AssignedToId;

            try
            {
                await _bugService.UpdateBugAsync(existingBug);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await _bugService.GetBugByIdAsync(id) == null) return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Помилка збереження: {ex.Message}");
                Bug = existingBug;
                await PopulateLists(existingBug.ProjectId);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        private async Task PopulateLists(int? projectId)
        {
            // Filter assignees to project members if a project is selected
            if (projectId.HasValue)
            {
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
                var all = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
                UsersList = new SelectList(all, "Id", "UserName");
            }

            // Show only accessible projects
            var projectsQuery = _context.Projects.AsQueryable();
            projectsQuery = await _bugAccessService.ApplyProjectFilterAsync(User, projectsQuery);
            var projects = await projectsQuery.OrderBy(p => p.Name).ToListAsync();
            ProjectsList = new SelectList(projects, "Id", "Name");
        }
    }
}