using System.Security.Claims;
using BugTrackingSystem.Data;
using BugTrackingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTrackingSystem.Services
{
    /// <inheritdoc cref="IBugAccessService"/>
    public class BugAccessService : IBugAccessService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BugAccessService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<bool> CanAccessAsync(ClaimsPrincipal userPrincipal, Bug bug)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            // Адмін має доступ до всього
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return true;

            var userId = user.Id;

            // Баг без проєкту: доступний лише автору та виконавцю
            if (bug.ProjectId == null)
                return bug.AuthorId == userId || bug.AssignedToId == userId;

            var isMember = await _context.ProjectMembers
                .AnyAsync(pm => pm.ProjectId == bug.ProjectId && pm.UserId == userId);

            // Менеджер бачить усі баги у своїх проєктах
            if (await _userManager.IsInRoleAsync(user, AppRoles.Manager))
                return isMember;

            // Тестувальник / Розробник: лише свої баги у своїх проєктах
            return isMember && (bug.AuthorId == userId || bug.AssignedToId == userId);
        }

        /// <inheritdoc/>
        public async Task<bool> CanDeleteAsync(ClaimsPrincipal userPrincipal, Bug bug)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return true;

            // Лише автор може видалити свій баг
            return bug.AuthorId == user.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> CanManageProjectAsync(ClaimsPrincipal userPrincipal, int projectId)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return false;

            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return true;

            // Менеджер управляє лише проєктами, де він є учасником
            if (await _userManager.IsInRoleAsync(user, AppRoles.Manager))
                return await _context.ProjectMembers
                    .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == user.Id);

            return false;
        }

        /// <inheritdoc/>
        public async Task<IQueryable<Bug>> ApplyAccessFilterAsync(ClaimsPrincipal userPrincipal, IQueryable<Bug> query)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return query.Where(_ => false);

            // Адмін бачить усі баги
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return query;

            var userId = user.Id;

            // Підзапит: проєкти, де користувач є учасником
            var memberProjectIds = _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.ProjectId);

            // Менеджер: усі баги у своїх проєктах
            if (await _userManager.IsInRoleAsync(user, AppRoles.Manager))
                return query.Where(b => b.ProjectId != null && memberProjectIds.Contains(b.ProjectId.Value));

            // Тестувальник / Розробник: лише свої баги у своїх проєктах
            return query.Where(b =>
                (b.AuthorId == userId || b.AssignedToId == userId) &&
                b.ProjectId != null &&
                memberProjectIds.Contains(b.ProjectId.Value));
        }

        /// <inheritdoc/>
        public async Task<IQueryable<Project>> ApplyProjectFilterAsync(ClaimsPrincipal userPrincipal, IQueryable<Project> query)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return query.Where(_ => false);

            // Адмін бачить усі проєкти
            if (await _userManager.IsInRoleAsync(user, AppRoles.Admin)) return query;

            var userId = user.Id;

            // Усі інші бачать лише свої проєкти (де є учасниками)
            var memberProjectIds = _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.ProjectId);

            return query.Where(p => memberProjectIds.Contains(p.Id));
        }
    }
}
