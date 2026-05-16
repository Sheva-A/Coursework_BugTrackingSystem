using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    /// <summary>
    /// Сутність бага (помилки) в системі відстеження.
    /// </summary>
    public class Bug
    {
        /// <summary>Унікальний ідентифікатор.</summary>
        public int Id { get; set; }

        /// <summary>Коротка назва бага (до 200 символів).</summary>
        [Required(ErrorMessage = "Вкажіть назву багу")]
        [StringLength(200, ErrorMessage = "Назва не може перевищувати 200 символів")]
        public string Title { get; set; }

        /// <summary>Детальний опис проблеми.</summary>
        [Required(ErrorMessage = "Опис є обов'язковим")]
        public string Description { get; set; }

        /// <summary>Поточний статус бага.</summary>
        public BugStatus Status { get; set; } = BugStatus.New;

        /// <summary>Пріоритет бага.</summary>
        public BugPriority Priority { get; set; } = BugPriority.Medium;

        /// <summary>Дата та час створення (UTC).</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Ідентифікатор автора бага.</summary>
        [Required]
        public string AuthorId { get; set; }

        /// <summary>Автор бага.</summary>
        public virtual ApplicationUser Author { get; set; }

        /// <summary>Ідентифікатор виконавця (може бути відсутнім).</summary>
        public string? AssignedToId { get; set; }

        /// <summary>Виконавець бага.</summary>
        public virtual ApplicationUser? AssignedTo { get; set; }

        /// <summary>Ідентифікатор пов'язаного проєкту (необов'язково).</summary>
        public int? ProjectId { get; set; }

        /// <summary>Пов'язаний проєкт.</summary>
        public virtual Project? Project { get; set; }
    }
}