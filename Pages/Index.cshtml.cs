using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;

namespace BugTrackingSystem.Pages
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

        /// <summary>Ім'я поточного користувача для вітання.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Активні завдання: баги, де поточний користувач є виконавцем
        /// і статус ще не «Закрито» або «Вирішено».
        /// </summary>
        public List<Bug> ActiveTasks { get; set; } = new();

        /// <summary>Проєкти, доступні поточному користувачу, з підрахунком багів.</summary>
        public List<ProjectSummary> MyProjects { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            UserName = user.UserName ?? user.Email ?? "Користувач";

            // Отримуємо доступні проєкти з урахуванням ролі
            var projectsQuery = _context.Projects.AsQueryable();
            projectsQuery = await _bugAccessService.ApplyProjectFilterAsync(User, projectsQuery);
            var projects = await projectsQuery.ToListAsync();

            // Отримуємо доступні баги з урахуванням ролі
            var bugsQuery = _context.Bugs
                .Include(b => b.Project)
                .AsQueryable();
            bugsQuery = await _bugAccessService.ApplyAccessFilterAsync(User, bugsQuery);
            var accessibleBugs = await bugsQuery.ToListAsync();

            // Мої актуальні завдання: призначені мені, не закриті та не вирішені
            ActiveTasks = accessibleBugs
                .Where(b => b.AssignedToId == user.Id
                         && b.Status != BugStatus.Resolved
                         && b.Status != BugStatus.Closed)
                .OrderByDescending(b => b.Priority)
                .Take(10)
                .ToList();

            // Міні-статистика по кожному проєкту
            MyProjects = projects.Select(p =>
            {
                var projectBugs = accessibleBugs.Where(b => b.ProjectId == p.Id).ToList();
                return new ProjectSummary
                {
                    Project    = p,
                    TotalBugs  = projectBugs.Count,
                    OpenBugs   = projectBugs.Count(b =>
                        b.Status == BugStatus.New || b.Status == BugStatus.InProgress)
                };
            }).ToList();
        }

        /// <summary>Зведені дані проєкту для відображення на дашборді.</summary>
        public class ProjectSummary
        {
            public Project Project  { get; set; } = null!;
            public int TotalBugs   { get; set; }
            public int OpenBugs    { get; set; }
        }
    }
}
