using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages.Projects
{
    [Authorize(Roles = "Admin,Manager")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IBugAccessService _bugAccessService;

        public DeleteModel(ApplicationDbContext context, IBugAccessService bugAccessService)
        {
            _context = context;
            _bugAccessService = bugAccessService;
        }

        public Project Project { get; set; } = default!;
        public int BugCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!await _bugAccessService.CanManageProjectAsync(User, id))
                return Forbid();

            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            Project = project;
            BugCount = await _context.Bugs.CountAsync(b => b.ProjectId == id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!await _bugAccessService.CanManageProjectAsync(User, id))
                return Forbid();

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            // Unlink bugs (set ProjectId = null) — bugs are not deleted
            var projectBugs = await _context.Bugs.Where(b => b.ProjectId == id).ToListAsync();
            foreach (var bug in projectBugs)
                bug.ProjectId = null;

            // Members are cascade-deleted by the FK, no need to remove manually
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
