using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.Especificaciones;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Diagnostics;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;

        [BindProperty]
        public CarroComprasVM CarroCompraVM { get; set; }

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;

        }

        public async Task<IActionResult> Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {
            //controlar sesiom
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var carro = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == claim.Value);
                var numeroProductos = carro.Count(); //numero de registros
                HttpContext.Session.SetInt32(Ds.ssCarroCompras, numeroProductos);
            }

            if (!string.IsNullOrEmpty(busqueda))
            {
                pageNumber = 1;
            }
            else
            {
                busqueda = busquedaActual.Trim();
            }
            ViewData["BusquedaActual"] = busqueda;

            if (pageNumber < 1) { pageNumber = 1; }

            Parametros parametros = new Parametros()
            {
                Pagesize = 4,
                PageNumber = pageNumber
            };

            PagedList<Producto> resultado;

            if (string.IsNullOrEmpty(busqueda))
            {
                resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros);
            }
            else
            {
                resultado = _unidadTrabajo.Producto.ObtenerTodosPaginado(parametros, p => p.Descripcion.Contains(busqueda));
            }

            ViewData["TotalPaginas"] = resultado.MetaData.TotalPages;
            ViewData["TotalRegistros"] = resultado.MetaData.TotalCount;
            ViewData["PageSize"] = resultado.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previo"] = "disabled";
            ViewData["Siguiente"] = "";

            if (pageNumber > 1) { ViewData["Previo"] = ""; }
            if (resultado.MetaData.TotalPages <= pageNumber) { ViewData["Siguiente"] = "disabled"; }

            return View(resultado);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            CarroCompraVM = new CarroComprasVM();
            CarroCompraVM.Compañia = await _unidadTrabajo.Compañia.ObtenerPrimero();
            CarroCompraVM.Producto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == id, incluirPropiedades: "Marca,Categoria");
            var bodegaProducto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(b => b.ProductoId == id && b.BodegaId == CarroCompraVM.Compañia.BodegaVentaId);

            if (bodegaProducto == null)
            {

                CarroCompraVM.Stock = 0;
            }
            else
            {
                CarroCompraVM.Stock = bodegaProducto.Cantidad;
            }
            CarroCompraVM.CarroCompra = new CarroCompra()
            {
                Producto = CarroCompraVM.Producto,
                ProductoId = CarroCompraVM.Producto.Id

            };
            return View(CarroCompraVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Detalle(CarroComprasVM carroComprasVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            carroComprasVM.CarroCompra.UsuarioaplicacionId = claim.Value;
            CarroCompra carroBD = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.UsuarioaplicacionId == claim.Value && c.ProductoId == carroComprasVM.CarroCompra.ProductoId);

            if (carroBD == null)
            {
                await _unidadTrabajo.CarroCompra.Agregar(carroComprasVM.CarroCompra);
            }
            else
            {
                carroBD.Cantidad += carroComprasVM.CarroCompra.Cantidad;
                _unidadTrabajo.CarroCompra.Actualizar(carroBD);

            }
            await _unidadTrabajo.Guardar();
            TempData[Ds.Exitosa] = "producto agregado al carrito";
            //se agrega sesion
            var carro = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == claim.Value);
            var numeroProductos = carro.Count(); //numero de registros

            HttpContext.Session.SetInt32(Ds.ssCarroCompras, numeroProductos);

            return RedirectToAction("Index");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
