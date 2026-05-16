using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Pages.Admin.Users
{
    /// <summary>
    /// Сторінка списку всіх користувачів (лише для адміністратора).
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<UserRow> Users { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allUsers = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var authoredBugs = await _context.Bugs.CountAsync(b => b.AuthorId == user.Id);
                var assignedBugs = await _context.Bugs.CountAsync(b => b.AssignedToId == user.Id);
                var projectCount = await _context.ProjectMembers.CountAsync(pm => pm.UserId == user.Id);

                Users.Add(new UserRow
                {
                    User          = user,
                    Roles         = roles.ToList(),
                    AuthoredBugs  = authoredBugs,
                    AssignedBugs  = assignedBugs,
                    ProjectCount  = projectCount
                });
            }
        }

        /// <summary>Рядок таблиці користувачів.</summary>
        public class UserRow
        {
            public ApplicationUser User { get; set; } = null!;
            public List<string> Roles  { get; set; } = new();
            public int AuthoredBugs   { get; set; }
            public int AssignedBugs   { get; set; }
            public int ProjectCount   { get; set; }
        }
    }
}
