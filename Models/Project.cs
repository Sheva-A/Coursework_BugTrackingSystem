using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    /// <summary>
    /// Проєкт — контейнер для пов'язаних багів та команди виконавців.
    /// </summary>
    public class Project
    {
        /// <summary>Унікальний ідентифікатор.</summary>
        public int Id { get; set; }

        /// <summary>Назва проєкту.</summary>
        [Required(ErrorMessage = "Вкажіть назву проєкту")]
        public string Name { get; set; }

        /// <summary>Короткий опис проєкту.</summary>
        [Required(ErrorMessage = "Вкажіть опис проєкту")]
        public string Description { get; set; }

        /// <summary>Баги, прив'язані до цього проєкту.</summary>
        public virtual ICollection<Bug> Bugs { get; set; } = new List<Bug>();

        /// <summary>Учасники проєкту.</summary>
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }
}