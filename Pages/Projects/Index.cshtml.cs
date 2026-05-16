using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages.Projects
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IBugAccessService _bugAccessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, IBugAccessService bugAccessService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _bugAccessService = bugAccessService;
            _userManager = userManager;
        }

        public IList<Project> Projects { get; set; } = default!;
        public HashSet<int> ManageableProjectIds { get; set; } = new();
        public bool IsAdminOrManager { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Projects.AsQueryable();
            query = await _bugAccessService.ApplyProjectFilterAsync(User, query);
            Projects = await query.ToListAsync();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                IsAdminOrManager =
                    await _userManager.IsInRoleAsync(currentUser, AppRoles.Admin) ||
                    await _userManager.IsInRoleAsync(currentUser, AppRoles.Manager);
            }

            foreach (var project in Projects)
                if (await _bugAccessService.CanManageProjectAsync(User, project.Id))
                    ManageableProjectIds.Add(project.Id);
        }
    }
}