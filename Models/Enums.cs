using System.ComponentModel.DataAnnotations;

namespace BugTrackingSystem.Models
{
    /// <summary>Статус бага у процесі обробки.</summary>
    public enum BugStatus
    {
        /// <summary>Щойно створений, ще не взятий в роботу.</summary>
        [Display(Name = "Новий")]
        New,

        /// <summary>Зараз виконується.</summary>
        [Display(Name = "В роботі")]
        InProgress,

        /// <summary>Виконавець позначив як вирішений.</summary>
        [Display(Name = "Вирішено")]
        Resolved,

        /// <summary>Повністю завершений і закритий.</summary>
        [Display(Name = "Закрито")]
        Closed
    }

    /// <summary>Пріоритет бага.</summary>
    public enum BugPriority
    {
        /// <summary>Не потребує негайної уваги.</summary>
        [Display(Name = "Низький")]
        Low,

        /// <summary>Стандартний пріоритет.</summary>
        [Display(Name = "Середній")]
        Medium,

        /// <summary>Потребує першочергового вирішення.</summary>
        [Display(Name = "Високий")]
        High
    }
}