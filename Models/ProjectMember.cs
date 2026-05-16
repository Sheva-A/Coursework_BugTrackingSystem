namespace BugTrackingSystem.Models
{
    /// <summary>
    /// Зв'язок «багато до багатьох» між <see cref="Project"/> та <see cref="ApplicationUser"/>.
    /// Визначає, які користувачі є учасниками конкретного проєкту.
    /// </summary>
    public class ProjectMember
    {
        /// <summary>Ідентифікатор проєкту (частина складеного PK).</summary>
        public int ProjectId { get; set; }

        /// <summary>Проєкт.</summary>
        public virtual Project Project { get; set; } = null!;

        /// <summary>Ідентифікатор користувача (частина складеного PK).</summary>
        public string UserId { get; set; } = null!;

        /// <summary>Користувач-учасник.</summary>
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
