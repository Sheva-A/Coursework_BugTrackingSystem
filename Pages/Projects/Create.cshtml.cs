using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Pages.Projects
{
    [Authorize(Roles = "Admin,Manager")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        /// <summary>ІД користувачів, обраних учасниками проєкту.</summary>
        [BindProperty]
        public List<string> SelectedUserIds { get; set; } = new();

        public List<ApplicationUser> AllUsers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            AllUsers = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllUsers = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
                return Page();
            }

            // Спочатку зберігаємо проєкт, щоб отримати його Id
            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            // Потім додаємо учасників
            foreach (var userId in SelectedUserIds)
            {
                _context.ProjectMembers.Add(new ProjectMember
                {
                    ProjectId = Project.Id,
                    UserId = userId
                });
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}