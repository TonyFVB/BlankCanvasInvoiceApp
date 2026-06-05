namespace BlankCanvasApp.Infrastructure
{

    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Supervisor = "Supervisor";
        public const string Auxiliar = "Auxiliar";
        public const string Cliente = "Cliente";

        public static readonly string[] All = { Admin, Supervisor, Auxiliar, Cliente };
    }

    public static class AppPermissions
    {
        //  Customers
        public const string VerClientes = "clientes.ver";
        public const string CrearClientes = "clientes.crear";
        public const string EditarClientes = "clientes.editar";
        public const string EliminarClientes = "clientes.eliminar";

        // Invoices 
        public const string VerFacturas = "facturas.ver";
        public const string CrearFacturas = "facturas.crear";
        public const string EliminarFacturas = "facturas.eliminar";
        public const string DescargarFacturas = "facturas.descargar";

        // Profile
        public const string EditarPerfil = "perfil.editar";

        // Users 
        public const string GestionarUsuarios = "usuarios.gestionar";

        // Export
        public const string ExportarFacturas = "facturas.exportar";

        /// Role-based permissions used in the Seeder.
        public static readonly Dictionary<string, string[]> PorRol = new()
        {
            [AppRoles.Admin] = new[]
            {
                VerClientes, CrearClientes, EditarClientes, EliminarClientes,
                VerFacturas, CrearFacturas, EliminarFacturas, DescargarFacturas, ExportarFacturas,
                EditarPerfil,
                GestionarUsuarios,
            },

            [AppRoles.Supervisor] = new[]
            {
                VerClientes, CrearClientes, EditarClientes,
                VerFacturas, DescargarFacturas, ExportarFacturas,
            },

            [AppRoles.Auxiliar] = new[]
            {
                VerClientes,
                VerFacturas, CrearFacturas, DescargarFacturas, ExportarFacturas,
            },

            [AppRoles.Cliente] = new[]
            {
                VerFacturas,           // solo sus propias — filtrado en el controlador
                DescargarFacturas,     // solo sus propias
                ExportarFacturas,      // solo sus propias
                EditarPerfil,          // solo su propio perfil
            },
        };
    }
}