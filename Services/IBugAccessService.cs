using System.Security.Claims;
using BugTrackingSystem.Models;

namespace BugTrackingSystem.Services
{
    /// <summary>
    /// Сервіс перевірки прав доступу до багів та проєктів.
    /// </summary>
    public interface IBugAccessService
    {
        /// <summary>
        /// Перевіряє, чи може користувач переглядати або редагувати баг.
        /// Адмін — завжди; менеджер — якщо він учасник проєкту;
        /// тестувальник/розробник — якщо є автором або виконавцем у своєму проєкті.
        /// </summary>
        Task<bool> CanAccessAsync(ClaimsPrincipal userPrincipal, Bug bug);

        /// <summary>
        /// Перевіряє, чи може користувач видалити баг.
        /// Дозволено лише адміністратору або автору бага.
        /// </summary>
        Task<bool> CanDeleteAsync(ClaimsPrincipal userPrincipal, Bug bug);

        /// <summary>
        /// Перевіряє, чи може користувач створювати, редагувати або видаляти проєкт.
        /// Адмін — будь-який проєкт; менеджер — лише свої проєкти.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проєкту.</param>
        Task<bool> CanManageProjectAsync(ClaimsPrincipal userPrincipal, int projectId);

        /// <summary>
        /// Повертає відфільтрований запит до багів відповідно до ролі користувача.
        /// </summary>
        Task<IQueryable<Bug>> ApplyAccessFilterAsync(ClaimsPrincipal userPrincipal, IQueryable<Bug> query);

        /// <summary>
        /// Повертає відфільтрований запит до проєктів відповідно до ролі користувача.
        /// Адмін бачить усі проєкти; інші — лише ті, де є учасниками.
        /// </summary>
        Task<IQueryable<Project>> ApplyProjectFilterAsync(ClaimsPrincipal userPrincipal, IQueryable<Project> query);
    }
}
