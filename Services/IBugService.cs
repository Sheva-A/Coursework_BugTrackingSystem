using BugTrackingSystem.Models;

namespace BugTrackingSystem.Services
{
    /// <summary>
    /// Сервіс для роботи з багами: отримання, створення, оновлення та видалення.
    /// </summary>
    public interface IBugService
    {
        /// <summary>
        /// Повертає запит до всіх багів (з можливим фільтром за проєктом).
        /// Включає навігаційні властивості: автор, виконавець, проєкт.
        /// </summary>
        /// <param name="projectId">Необов'язковий фільтр за проєктом.</param>
        IQueryable<Bug> GetBugsQuery(int? projectId = null);

        /// <summary>
        /// Повертає назву проєкту за його ідентифікатором.
        /// </summary>
        /// <param name="projectId">Ідентифікатор проєкту.</param>
        Task<string?> GetProjectNameAsync(int projectId);

        /// <summary>
        /// Повертає баг за ідентифікатором з усіма пов'язаними даними.
        /// </summary>
        /// <param name="id">Ідентифікатор бага.</param>
        Task<Bug?> GetBugByIdAsync(int id);

        /// <summary>
        /// Зберігає новий баг у базі даних.
        /// </summary>
        /// <param name="bug">Новий баг.</param>
        Task CreateBugAsync(Bug bug);

        /// <summary>
        /// Оновлює існуючий баг у базі даних.
        /// </summary>
        /// <param name="bug">Баг із оновленими даними.</param>
        Task UpdateBugAsync(Bug bug);

        /// <summary>
        /// Видаляє баг за ідентифікатором.
        /// </summary>
        /// <param name="id">Ідентифікатор бага для видалення.</param>
        Task DeleteBugAsync(int id);
    }
}