using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BugTrackingSystem.Extensions
{
    /// <summary>
    /// Розширення для перечислень (<see cref="Enum"/>).
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Повертає локалізовану назву значення перечислення з атрибута <see cref="DisplayAttribute"/>.
        /// Якщо атрибут відсутній — повертає стандартне <c>ToString()</c>.
        /// </summary>
        /// <param name="value">Значення перечислення.</param>
        /// <returns>Рядок з відображуваною назвою.</returns>
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            var attr = field.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }
    }
}
