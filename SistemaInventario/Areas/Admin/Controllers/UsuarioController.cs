using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Utilidades;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Ds.Role_Admin)]
    public class UsuarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly ApplicationDbContext _db;
        public UsuarioController(IUnidadTrabajo unidadTrabajo, ApplicationDbContext applicationDbContext)
        {
            _unidadTrabajo = unidadTrabajo;
            _db = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarioLista = await _unidadTrabajo.UsuarioAplicacion.ObtenerTodos();
            var userRole = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();

            foreach (var usr in usuarioLista)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == usr.Id).RoleId;
                usr.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
            }
            return Json(new { data = usuarioLista });

        }


        [HttpPost]
        public async Task<IActionResult> BloquearDesbloquear([FromBody] string id)
        {
            var usuario = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(p => p.Id == id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Erorr en el usuario" });
            }
            if (usuario.LockoutEnd != null && usuario.LockoutEnd > DateTime.Now)
            {
                usuario.LockoutEnd = DateTime.Now;
            }
            else
            {
                usuario.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            await _unidadTrabajo.Guardar();

            return Json(new { success = true, message = "Ooperacion exitosa" });
        }

        #endregion




    }
}
