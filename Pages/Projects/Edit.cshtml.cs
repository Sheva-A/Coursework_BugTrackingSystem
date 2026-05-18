using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages.Projects
{
    [Authorize(Roles = "Admin,Manager")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IBugAccessService _bugAccessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, IBugAccessService bugAccessService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _bugAccessService = bugAccessService;
            _userManager = userManager;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        [BindProperty]
        public List<string> SelectedUserIds { get; set; } = new();

        public List<ApplicationUser> AllUsers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await _bugAccessService.CanManageProjectAsync(User, id))
                return Forbid();

            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            Project = project;
            SelectedUserIds = project.Members.Select(m => m.UserId).ToList();
            AllUsers = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!await _bugAccessService.CanManageProjectAsync(User, id))
                return Forbid();

            var existingProject = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProject == null) return NotFound();

            if (!ModelState.IsValid)
            {
                Project = existingProject;
                AllUsers = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
                return Page();
            }

            existingProject.Name = Project.Name;
            existingProject.Description = Project.Description;

            // Синхронізуємо список учасників: видаляємо знятих, додаємо нових
            var currentMemberIds = existingProject.Members.Select(m => m.UserId).ToHashSet();
            var newMemberIds = SelectedUserIds.ToHashSet();

            foreach (var removed in currentMemberIds.Except(newMemberIds))
                existingProject.Members.Remove(existingProject.Members.First(m => m.UserId == removed));

            foreach (var added in newMemberIds.Except(currentMemberIds))
                existingProject.Members.Add(new ProjectMember { ProjectId = id, UserId = added });

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
