using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace BugTrackingSystem.Models
{
    /// <summary>
    /// Користувач системи відстеження багів.
    /// Розширює стандартний <see cref="IdentityUser"/> навігаційними властивостями.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>Баги, які створив цей користувач.</summary>
        public virtual ICollection<Bug> CreatedBugs { get; set; } = new List<Bug>();

        /// <summary>Баги, призначені на цього користувача.</summary>
        public virtual ICollection<Bug> AssignedBugs { get; set; } = new List<Bug>();

        /// <summary>Членство користувача у проєктах.</summary>
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    }
}