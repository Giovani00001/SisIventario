using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Security.Claims;

namespace SistemaInventario.Areas.Inventario.Controllers
{

    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        [BindProperty]
        public CarroComprasVM CarroCompraVM { get; set; }
        public CarroController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CarroCompraVM = new CarroComprasVM();
            CarroCompraVM.Orden = new Modelos.Orden();
            CarroCompraVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == claim.Value, incluirPropiedades: "Producto");
            CarroCompraVM.Orden.TotalOrden = 0;
            CarroCompraVM.Orden.UsuarioAplicacionId = claim.Value;

            foreach (var item in CarroCompraVM.CarroCompraLista)
            {
                item.Precio = item.Producto.Precio;// siempre se muestra el precio actual del producto
                CarroCompraVM.Orden.TotalOrden += (item.Precio * item.Cantidad);

            }

            return View(CarroCompraVM);
        }


        public async Task<IActionResult> mas(int carroId)
        {
            var carroCompra = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId);
            carroCompra.Cantidad += 1;
            await _unidadTrabajo.Guardar();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> menos(int carroId)
        {
            var carroCompra = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId);
            if (carroCompra.Cantidad == 1)
            {
                /// remover y actualizar sesion
                /// 
                var carroLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == carroCompra.UsuarioaplicacionId);

                var numProd = carroLista.Count();
                _unidadTrabajo.CarroCompra.Remover(carroCompra);
                await _unidadTrabajo.Guardar();
                HttpContext.Session.SetInt32(Ds.ssCarroCompras, numProd - 1);

            }
            else
            {
                carroCompra.Cantidad -= 1;
                await _unidadTrabajo.Guardar();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> remover(int carroId)
        {
            var carroCompra = await _unidadTrabajo.CarroCompra.ObtenerPrimero(c => c.Id == carroId);
            var carroLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == carroCompra.UsuarioaplicacionId);
            var numProd = carroLista.Count();
            _unidadTrabajo.CarroCompra.Remover(carroCompra);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(Ds.ssCarroCompras, numProd - 1);
            return RedirectToAction("Index");
        }
    }
}
