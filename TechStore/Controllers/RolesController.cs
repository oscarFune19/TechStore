using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string nombre)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                await _roleManager.CreateAsync(new IdentityRole(nombre.Trim()));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();

            if (rol.Name is "Admin" or "Vendedor" or "Cliente")
            {
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(rol);
            return RedirectToAction(nameof(Index));
        }
    }
}
