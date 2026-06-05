using Microsoft.AspNetCore.Authorization;

namespace BlackCanvasApp.Authorization
{
    /// Atributo de autorización basado en permisos.
    /// Uso: [HasPermission(AppPermissions.VerFacturas)]

    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
            : base(policy: permission)
        {
        }
    }
}