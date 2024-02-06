using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductoController(IUnidadTrabajo unidad, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidad;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto")
            };

            if (id == null)
            {
                ///// productoVM.Producto.Categoria = new();
                productoVM.Producto.Estado = true;
                //nuevo producto
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);

            }

        }

        #region API

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {


            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productoVM.Producto.Id == 0)
                {
                    //crear nuevo producto
                    string upload = webRootPath + Ds.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(file[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        file[0].CopyTo(fileStream);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    //Actualizar
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);

                    if (file.Count > 0)
                    {
                        string upload = webRootPath + Ds.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(file[0].FileName);

                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            file[0].CopyTo(fileStream);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);

                }
                TempData[Ds.Exitosa] = "Transaccion exitosa";
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
                ///return View(nameof(Index));
            }
            ///productoVM.PadreLista= _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto");
            // productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria");
            ///// productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca");
            return View(productoVM);
        }











        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");
            return Json(new { data = todos });
        }




        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var ProductoBD = await _unidadTrabajo.Producto.Obtener(id);
            if (ProductoBD == null)
            {
                return Json(new { success = false, message = "Error al obtener el producto" });
            }

            string upload = _webHostEnvironment.WebRootPath + Ds.ImagenRuta;
            var anteriorFile = Path.Combine(upload, ProductoBD.ImagenUrl);

            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }


            _unidadTrabajo.Producto.Remover(ProductoBD);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto eliminado correctamente" });
        }


        [ActionName("ValidaSerie")]
        public async Task<IActionResult> ValidaSerie(string serie, int id = 0)
        {
            if (id == 0)
            {
                return Json(new
                {
                    data = (await _unidadTrabajo.Producto.ObtenerTodos())
                .Any(b => b.NumSerie.Trim().ToLower() == serie.Trim().ToLower())
                });
            }
            return Json(new
            {
                data = (await _unidadTrabajo.Producto.ObtenerTodos())
            .Any(b => b.NumSerie.Trim().ToLower() == serie.Trim().ToLower() && b.Id != id)
            });
        }
        #endregion
    }
}
