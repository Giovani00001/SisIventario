using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Migrations;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Utilidades;
using System.Security.Claims;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public OrdenDetalleVM _ordenDetalleVM { get; set; }

        public OrdenController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }




        [HttpGet]
        public async Task<IActionResult> ObtenerListta(string estado)
        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Orden> todos;


            if (User.IsInRole(Ds.Role_Admin))
            {
                todos = await _unidadTrabajo.Orden.ObtenerTodos(incluirPropiedades: "UsuarioAplicacion");
            }
            else
            {

                todos = await _unidadTrabajo.Orden.ObtenerTodos(u => u.UsuarioAplicacionId == claim.Value, incluirPropiedades: "UsuarioAplicacion");
            }
            // vaidar estado
            switch (estado)
            {
                case "aprobado":
                    todos = todos.ToList().Where(o => o.EstadoOrden == Ds.EstadoAprobado);
                    break;
                case "completado":
                    todos = todos.ToList().Where(o => o.EstadoOrden == Ds.EstadoEnviado);
                    break;
                default:
                    break;

            }
            return Json(new { data = todos });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            _ordenDetalleVM = new OrdenDetalleVM()
            {
                Orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion"),
                OrdenDetalleLista = await _unidadTrabajo.OrdenDetalle.ObtenerTodos(o => o.OrdenId == id, incluirPropiedades: "Producto")
            };
            return View(_ordenDetalleVM);
        }
        [Authorize]
        public async Task<IActionResult> Procesar(int id)
        {
            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id);
            orden.EstadoOrden = Ds.EstadoProceso;
            await _unidadTrabajo.Guardar();
            TempData[Ds.Exitosa] = "Orden cambiado a estado en proceso";

            return RedirectToAction("Detalle", new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = Ds.Role_Admin)]
        public async Task<IActionResult> EnviarOrden(OrdenDetalleVM ordenDetalleVM)
        {
            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == ordenDetalleVM.Orden.Id);
            orden.EstadoOrden = Ds.EstadoEnviado;
            orden.Carrier = ordenDetalleVM.Orden.Carrier;
            orden.NumeroEnvio = ordenDetalleVM.Orden.NumeroEnvio;
            orden.FechaEnvio = DateTime.Now;
            await _unidadTrabajo.Guardar();

            TempData[Ds.Exitosa] = "Orden cambiado a estado enviado";

            return RedirectToAction("Detalle", new { id = ordenDetalleVM.Orden.Id });
        }
    }
}
