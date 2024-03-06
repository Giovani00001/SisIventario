
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Stripe.Issuing;
using System.Security.Claims;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace SistemaInventario.Areas.Inventario.Controllers
{

    [Area("Inventario")]
    public class CarroController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private string _weUrl;


        [BindProperty]
        public CarroComprasVM CarroCompraVM { get; set; }
        public CarroController(IUnidadTrabajo unidadTrabajo, IConfiguration cong)
        {
            _unidadTrabajo = unidadTrabajo;
            _weUrl = cong.GetValue<string>("DomainUrl:WEV_URL");
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

        public async Task<IActionResult> Proceder()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CarroCompraVM = new CarroComprasVM()
            {
                Orden = new Orden(),
                CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(
                    c => c.UsuarioaplicacionId == claim.Value, incluirPropiedades: "Producto"),
                Compañia = await _unidadTrabajo.Compañia.ObtenerPrimero()
            };
            CarroCompraVM.Orden.TotalOrden = 0;
            CarroCompraVM.Orden.UsuarioAplicacion = await _unidadTrabajo.UsuarioAplicacion.ObtenerPrimero(u => u.Id == claim.Value);

            foreach (var item in CarroCompraVM.CarroCompraLista)
            {
                item.Precio = item.Producto.Precio;
                CarroCompraVM.Orden.TotalOrden += (item.Precio * item.Cantidad);
            }
            CarroCompraVM.Orden.NombreCliente = CarroCompraVM.Orden.UsuarioAplicacion.Nombres +
                "" + CarroCompraVM.Orden.UsuarioAplicacion.Apellidos;
            CarroCompraVM.Orden.Telefono = CarroCompraVM.Orden.UsuarioAplicacion.PhoneNumber;
            CarroCompraVM.Orden.Direccion = CarroCompraVM.Orden.UsuarioAplicacion.Direccion;
            CarroCompraVM.Orden.Pais = CarroCompraVM.Orden.UsuarioAplicacion.Pais;
            CarroCompraVM.Orden.Ciudad = CarroCompraVM.Orden.UsuarioAplicacion.Ciudad;


            foreach (var item in CarroCompraVM.CarroCompraLista)
            {
                var producto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(b => b.ProductoId == item.ProductoId &&
                                                                         b.BodegaId == CarroCompraVM.Compañia.BodegaVentaId);

                if (item.Cantidad > producto.Cantidad)
                {
                    TempData[Ds.Error] = "la cantidad del producto " + item.Producto.Descripcion + " Excede al stock actual " +
                        producto.Cantidad;
                    return RedirectToAction("Index");
                }
            }
            return View(CarroCompraVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Proceder(CarroComprasVM carroComprasVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            carroComprasVM.CarroCompraLista = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == claim.Value,
                incluirPropiedades: "Producto");

            carroComprasVM.Compañia = await _unidadTrabajo.Compañia.ObtenerPrimero();
            carroComprasVM.Orden.TotalOrden = 0;
            carroComprasVM.Orden.UsuarioAplicacionId = claim.Value;
            carroComprasVM.Orden.FechaOrden = DateTime.Now;

            foreach (var item in carroComprasVM.CarroCompraLista)
            {
                item.Precio = item.Producto.Precio;
                carroComprasVM.Orden.TotalOrden += (item.Precio * item.Cantidad);
            }
            foreach (var item in carroComprasVM.CarroCompraLista)
            {
                var producto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(b => b.ProductoId == item.ProductoId &&
                                                                        b.BodegaId == carroComprasVM.Compañia.BodegaVentaId);

                if (item.Cantidad > producto.Cantidad)
                {
                    TempData[Ds.Error] = "la cantidad del producto " + item.Producto.Descripcion + " Excede al stock actual " +
                        producto.Cantidad;
                    return RedirectToAction("Index");
                }
            }

            carroComprasVM.Orden.EstadoOrden = Ds.EstadoPendiente;
            carroComprasVM.Orden.EstadoPago = Ds.PagoEstadoPendiente;

            await _unidadTrabajo.Orden.Agregar(carroComprasVM.Orden);
            await _unidadTrabajo.Guardar();

            //detalle

            foreach (var item in carroComprasVM.CarroCompraLista)
            {
                OrdenDetalle ordenDetalle = new OrdenDetalle()
                {
                    ProdutoId = item.ProductoId,
                    OrdenId = carroComprasVM.Orden.Id,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad
                };
                await _unidadTrabajo.OrdenDetalle.Agregar(ordenDetalle);
                await _unidadTrabajo.Guardar();
            }
            //stripe
            var options = new SessionCreateOptions
            {

                SuccessUrl = _weUrl + $"inventario/carro/OrdenConfirmacion?id={carroComprasVM.Orden.Id}",
                CancelUrl = _weUrl + "Inventario/Carro/index",

                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>()

            };
            foreach (var item in carroComprasVM.CarroCompraLista)
            {
                var sessionLineItems = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Precio * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Producto.Descripcion
                        }

                    },
                    Quantity = item.Cantidad

                };
                options.LineItems.Add(sessionLineItems);
            }
            var services = new SessionService();
            Stripe.Checkout.Session session = services.Create(options);
            _unidadTrabajo.Orden.ActualizarPagoStripeId(carroComprasVM.Orden.Id, session.Id, session.PaymentIntentId);
            await _unidadTrabajo.Guardar();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(StatusCodes.Status303SeeOther);
        }

        public async Task<IActionResult> OrdenConfirmacion(int id)
        {

            var orden = await _unidadTrabajo.Orden.ObtenerPrimero(o => o.Id == id, incluirPropiedades: "UsuarioAplicacion");
            var service = new SessionService();
            Stripe.Checkout.Session sesion = service.Get(orden.SessionId);
            var carroCompra = await _unidadTrabajo.CarroCompra.ObtenerTodos(c => c.UsuarioaplicacionId == orden.UsuarioAplicacionId);
            if (sesion.PaymentStatus.ToLower() == "paid")
            {
                _unidadTrabajo.Orden.ActualizarPagoStripeId(id, sesion.Id, sesion.PaymentIntentId);
                _unidadTrabajo.Orden.ActualizarEstado(id, Ds.EstadoAprobado, Ds.PagoEstadoAprobado);
                await _unidadTrabajo.Guardar();

                //disinuir Stock
                var comañia = await _unidadTrabajo.Compañia.ObtenerPrimero();

                foreach (var item in carroCompra)
                {
                    var bodegaProducto = new BodegaProducto();
                    bodegaProducto = await _unidadTrabajo.BodegaProducto.ObtenerPrimero(b => b.ProductoId == item.ProductoId && b.BodegaId == comañia.BodegaVentaId);
                    await _unidadTrabajo.KardexInventario.RegitrarKardex(bodegaProducto.Id, "Salida",
                                                                           "Venta de la orden " + id, bodegaProducto.Cantidad, item.Cantidad, orden.UsuarioAplicacionId);

                    bodegaProducto.Cantidad -= item.Cantidad;
                    await _unidadTrabajo.Guardar();
                }


            }

            //borramos el carro de compras y su sesion


            List<CarroCompra> carroCompralista = carroCompra.ToList();
            _unidadTrabajo.CarroCompra.RemoverRango(carroCompralista);
            await _unidadTrabajo.Guardar();
            HttpContext.Session.SetInt32(Ds.ssCarroCompras, 0);

            return View(id);
        }
    }

}

