using Microsoft.AspNetCore.Identity;

namespace BlankCanvasApp.Infrastructure
{
    /// <summary>
    /// Usuario del sistema — extiende IdentityUser con campos de negocio.
    /// La tabla generada en BD será "AspNetUsers" con columnas adicionales.
    /// </summary>
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}".Trim();

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }
    }
}
