using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    [Authorize(Roles = Ds.Role_Admin + "," + Ds.Role_Inventario)]
    public class InventarioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public InventarioVM inventarioVM { get; set; }
        public InventarioController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NuevoInventario()
        {
            inventarioVM = new InventarioVM()
            {
                Inventario = new Modelos.Inventario(),
                BodegaLista = _unidadTrabajo.Inventario.ObtenertodosDropDownList("Bodega")

            };

            inventarioVM.Inventario.Estado = false;
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            inventarioVM.Inventario.UsuarioAplicacionId = claim.Value;
            inventarioVM.Inventario.Fechainicial = DateTime.Now;
            inventarioVM.Inventario.FechaFinal = DateTime.Now;

            return View(inventarioVM);

        }

        public async Task<IActionResult> DetalleInventario(int id)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.ObtenerPrimero(i => i.Id == id, incluirPropiedades: "Bodega");
            inventarioVM.InventarioDetalles = await _unidadTrabajo.InventarioDetalle.ObtenerTodos(i => i.InventarioId == id, incluirPropiedades: "Producto,Producto.Marca");
            return View(inventarioVM);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NuevoInventario(InventarioVM inventarioVM)
        {
            if (ModelState.IsValid)
            {
                inventarioVM.Inventario.Fechainicial = DateTime.Now;
                inventarioVM.Inventario.FechaFinal = DateTime.Now;
                await _unidadTrabajo.Inventario.Agregar(inventarioVM.Inventario);
                await _unidadTrabajo.Guardar();
                return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
            }
            inventarioVM.BodegaLista = _unidadTrabajo.Inventario.ObtenertodosDropDownList("Bodega");
            return View(inventarioVM);
        }


        public async Task<IActionResult> GenerarStock(int id)
        {

            var inventario = await _unidadTrabajo.Inventario.Obtener(id);
            var detalleLista = await _unidadTrabajo.InventarioDetalle.ObtenerTodos(d => d.InventarioId == id);
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var d in detalleLista)
            {
                var bodegaProducto = new BodegaProducto();
                bodegaProducto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(x => x.ProductoId == d.Productoid && x.BodegaId == inventario.BodegaId);
                if (bodegaProducto != null)
                {
                    await _unidadTrabajo.KardexInventario.RegitrarKardex(bodegaProducto.Id, "Entrada", "Registro de Inventario", bodegaProducto.Cantidad, d.Cantidad, claim.Value);
                    bodegaProducto.Cantidad += d.Cantidad;
                    await _unidadTrabajo.Guardar();
                }
                else
                {
                    bodegaProducto = new BodegaProducto();
                    bodegaProducto.BodegaId = inventario.BodegaId;
                    bodegaProducto.ProductoId = d.Productoid;
                    bodegaProducto.Cantidad = d.Cantidad;

                    await _unidadTrabajo.BodegaProducto.Agregar(bodegaProducto);
                    await _unidadTrabajo.Guardar();
                    await _unidadTrabajo.KardexInventario.RegitrarKardex(bodegaProducto.Id, "Entrada", "Inventario Inicial", 0, d.Cantidad, claim.Value);

                }

            }
            // actualizar cabecera
            inventario.Estado = true;
            inventario.Fechainicial = DateTime.Now;
            await _unidadTrabajo.Guardar();
            TempData[Ds.Exitosa] = "Stock generado con exito";
            return RedirectToAction("Index");

        }

        public IActionResult KardexProducto()
        {
            return View();
        }

        [HttpPost]
        public IActionResult KardexProducto(string fechaInicioId, string fechaFinalId, int productoId)
        {
            return RedirectToAction("KardexProductoResultado", new { fechaInicioId, fechaFinalId, productoId });
        }

        #region API


        public async Task<IActionResult> KardexProductoResultado(string fechaInicioId, string fechaFinalId, int productoId)
        {
            KardexInventarioVM kardexInventarioVM = new();
            kardexInventarioVM.Producto = new Producto();
            kardexInventarioVM.Producto = await _unidadTrabajo.Producto.Obtener(productoId);

            kardexInventarioVM.FechaInicio = DateTime.Parse(fechaInicioId);
            kardexInventarioVM.FechaFinal = DateTime.Parse(fechaFinalId).AddHours(23).AddMinutes(59);

            kardexInventarioVM.KardexInventarioLista = await _unidadTrabajo.KardexInventario.ObtenerTodos(
                k => k.BodegaProducto.ProductoId == productoId && (k.FechaRegistro >= kardexInventarioVM.FechaInicio && k.FechaRegistro <= kardexInventarioVM.FechaFinal),
                incluirPropiedades: "BodegaProducto,BodegaProducto.Producto,BodegaProducto.Bodega",
                orderBy: o=>o.OrderBy(o=>o.FechaRegistro)
                );
            return View(kardexInventarioVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetalleInventario(int InventarioId, int productoId, int cantidadId)
        {
            inventarioVM = new InventarioVM();
            inventarioVM.Inventario = await _unidadTrabajo.Inventario.ObtenerPrimero(p => p.Id == InventarioId);
            var bodegaProducto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(p => p.ProductoId == productoId && p.BodegaId == inventarioVM.Inventario.BodegaId);
            var detalle = await _unidadTrabajo.InventarioDetalle.ObtenerPrimero(d => d.InventarioId == InventarioId && d.Productoid == productoId);

            if (detalle == null)
            {
                inventarioVM.InventarioDetalle = new InventarioDetalle();
                inventarioVM.InventarioDetalle.Productoid = productoId;
                inventarioVM.InventarioDetalle.InventarioId = InventarioId;

                if (bodegaProducto != null)
                {
                    inventarioVM.InventarioDetalle.StockAnterior = bodegaProducto.Cantidad;
                }
                else
                {
                    inventarioVM.InventarioDetalle.StockAnterior = 0;
                }
                inventarioVM.InventarioDetalle.Cantidad = cantidadId;
                await _unidadTrabajo.InventarioDetalle.Agregar(inventarioVM.InventarioDetalle);
                await _unidadTrabajo.Guardar();
            }
            else
            {
                detalle.Cantidad += cantidadId;
                await _unidadTrabajo.Guardar();
            }
            return RedirectToAction("DetalleInventario", new { id = InventarioId });
        }



        public async Task<IActionResult> Mas(int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.Obtener(id);

            if (detalle != null)
            {
                inventarioVM.Inventario = await _unidadTrabajo.Inventario.Obtener(detalle.InventarioId);
                detalle.Cantidad += 1;
                await _unidadTrabajo.Guardar();

            }
            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }
        public async Task<IActionResult> Menos(int id)
        {
            inventarioVM = new InventarioVM();
            var detalle = await _unidadTrabajo.InventarioDetalle.Obtener(id);

            if (detalle != null)
            {
                inventarioVM.Inventario = await _unidadTrabajo.Inventario.Obtener(detalle.InventarioId);
                if (detalle.Cantidad == 1)
                {
                    _unidadTrabajo.InventarioDetalle.Remover(detalle);
                    await _unidadTrabajo.Guardar();
                }
                else
                {
                    detalle.Cantidad -= 1;
                    await _unidadTrabajo.Guardar();
                }

            }
            return RedirectToAction("DetalleInventario", new { id = inventarioVM.Inventario.Id });
        }



        [HttpGet]
        public async Task<IActionResult> BuscarProducto(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var listaProducto = await _unidadTrabajo.Producto.ObtenerTodos(p => p.Estado == true);
                var data = listaProducto.Where(p => p.NumSerie.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Descripcion.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(data);
            }
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.BodegaProducto.ObtenerTodos(incluirPropiedades: "Producto,Bodega,Producto.Marca");
            return Json(new { data = todos });

        }
        #endregion
    }
}
