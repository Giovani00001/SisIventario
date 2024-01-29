using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SistemaInventario.AccesoDatos.Repositorio;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MarcaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;


        public MarcaController(IUnidadTrabajo unidad)
        {
            _unidadTrabajo = unidad;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Marca Marca = new();
            if (id == null)
            {
                Marca.Estado = true;
                return View(Marca);

            }
            Marca = await _unidadTrabajo.Marca.Obtener(id.GetValueOrDefault());
            if (Marca == null)
            {
                return NotFound();
            }
            return View(Marca);

        }

        #region API

        [ActionName("ValidaNombre")]
        public async Task<IActionResult> ValidaNombre(string nombre, int id = 0)
        {
            if (id == 0)
            {
                return Json(new
                {
                    data = (await _unidadTrabajo.Marca.ObtenerTodos())
                .Any(b => b.Nombre.Trim().ToLower() == nombre.Trim().ToLower())
                });
            }
            return Json(new
            {
                data = (await _unidadTrabajo.Marca.ObtenerTodos())
            .Any(b => b.Nombre.Trim().ToLower() == nombre.Trim().ToLower() && b.Id != id)
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Marca.ObtenerTodos();
            return Json(new { data = todos });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Marca marca)
        {
            if (ModelState.IsValid)
            {
                if (marca.Id == 0)
                {
                    await _unidadTrabajo.Marca.Agregar(marca);
                    TempData[Ds.Exitosa] = "Categoria creeada con exito";
                }
                else
                {
                    _unidadTrabajo.Marca.Actualizar(marca);
                    TempData[Ds.Exitosa] = "Categoria actualizada correctamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[Ds.Error] = "Error en la operacion con la categoria";
            return View(marca);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var marcaBD = await _unidadTrabajo.Marca.Obtener(id);
            if (marcaBD == null)
            {
                return Json(new { success = false, message = "Error al obtener la categoria" });
            }
            _unidadTrabajo.Marca.Remover(marcaBD);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria Eliminada correctamevte" });
        }

        #endregion
    }
}
