using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BugTrackingSystem.Models;
using BugTrackingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Pages.Bugs
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IBugService _bugService;
        private readonly IBugAccessService _bugAccessService;

        public IndexModel(IBugService bugService, IBugAccessService bugAccessService)
        {
            _bugService = bugService;
            _bugAccessService = bugAccessService;
        }

        [BindProperty(SupportsGet = true)]
        public int? ProjectId { get; set; }

        public string? ProjectName { get; set; }

        public IEnumerable<Bug> Bugs { get; set; } = new List<Bug>();

        public HashSet<int> DeletableBugIds { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (ProjectId.HasValue)
                ProjectName = await _bugService.GetProjectNameAsync(ProjectId.Value);

            var query = _bugService.GetBugsQuery(ProjectId);

            query = await _bugAccessService.ApplyAccessFilterAsync(User, query);

            Bugs = await query.ToListAsync();

            foreach (var bug in Bugs)
                if (await _bugAccessService.CanDeleteAsync(User, bug))
                    DeletableBugIds.Add(bug.Id);
        }
    }
}