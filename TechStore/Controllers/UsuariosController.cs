using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechStore.Models;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuariosController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var lista = new List<UsuarioListadoItem>();
            foreach (var usuario in usuarios)
            {
                lista.Add(new UsuarioListadoItem
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName,
                    Email = usuario.Email,
                    Roles = await _userManager.GetRolesAsync(usuario)
                });
            }
            return View(lista);
        }

        public async Task<IActionResult> EditarRoles(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            var actuales = await _userManager.GetRolesAsync(usuario);
            var nombres = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToListAsync();

            return View(new EditarRolesUsuarioViewModel
            {
                UserId = usuario.Id,
                UserName = usuario.UserName,
                Roles = nombres.Select(n => new RolSeleccion { Nombre = n, Seleccionado = actuales.Contains(n) }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarRoles(EditarRolesUsuarioViewModel modelo)
        {
            var usuario = await _userManager.FindByIdAsync(modelo.UserId);
            if (usuario == null) return NotFound();

            var existentes = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
            var seleccionados = modelo.Roles
                .Where(r => r.Seleccionado && existentes.Contains(r.Nombre))
                .Select(r => r.Nombre)
                .ToList();
            var actuales = await _userManager.GetRolesAsync(usuario);

            await _userManager.RemoveFromRolesAsync(usuario, actuales.Except(seleccionados));
            await _userManager.AddToRolesAsync(usuario, seleccionados.Except(actuales));

            return RedirectToAction(nameof(Index));
        }
    }
}
