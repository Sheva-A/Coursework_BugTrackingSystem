namespace BugTrackingSystem.Models
{
    /// <summary>
    /// Константи ролей системи та зручні групування.
    /// </summary>
    public static class AppRoles
    {
        /// <summary>Повний доступ до всього.</summary>
        public const string Admin = "Admin";

        /// <summary>Керує проєктами та їхніми учасниками.</summary>
        public const string Manager = "Manager";

        /// <summary>Розробник, який виконує задачі.</summary>
        public const string Developer = "Developer";

        /// <summary>Тестувальник, який знаходить та відслідковує баги.</summary>
        public const string Tester = "Tester";

        /// <summary>Усі ролі системи.</summary>
        public static readonly string[] All = { Admin, Manager, Developer, Tester };

        /// <summary>Ролі з розширеним доступом (адміністратор і менеджер).</summary>
        public static readonly string[] Privileged = { Admin, Manager };
    }
}
