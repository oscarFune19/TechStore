namespace TechStore.Models
{
    public class UsuarioListadoItem
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class RolSeleccion
    {
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; }
    }

    public class EditarRolesUsuarioViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public List<RolSeleccion> Roles { get; set; } = new();
    }
}
