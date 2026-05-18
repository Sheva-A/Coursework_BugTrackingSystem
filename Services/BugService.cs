using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Services
{
    /// <inheritdoc cref="IBugService"/>
    public class BugService : IBugService
    {
        private readonly ApplicationDbContext _context;

        public BugService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public IQueryable<Bug> GetBugsQuery(int? projectId = null)
        {
            IQueryable<Bug> query = _context.Bugs
                .Include(b => b.Author)
                .Include(b => b.AssignedTo)
                .Include(b => b.Project);

            if (projectId.HasValue)
                query = query.Where(b => b.ProjectId == projectId.Value);

            return query;
        }

        /// <inheritdoc/>
        public async Task<string?> GetProjectNameAsync(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            return project?.Name;
        }

        /// <inheritdoc/>
        public async Task<Bug?> GetBugByIdAsync(int id)
        {
            return await _context.Bugs
                .Include(b => b.Author)
                .Include(b => b.AssignedTo)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        /// <inheritdoc/>
        public async Task CreateBugAsync(Bug bug)
        {
            _context.Add(bug);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task UpdateBugAsync(Bug bug)
        {
            _context.Update(bug);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task DeleteBugAsync(int id)
        {
            var bug = await _context.Bugs.FindAsync(id);
            if (bug == null) return;

            _context.Bugs.Remove(bug);
            await _context.SaveChangesAsync();
        }
    }
}