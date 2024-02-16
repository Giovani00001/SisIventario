using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Ds.Role_Admin)]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;


        public CategoriaController(IUnidadTrabajo unidad)
        {
            _unidadTrabajo = unidad;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria Categoria = new();
            if (id == null)
            {
                Categoria.Estado = true;
                return View(Categoria);

            }
            Categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (Categoria == null)
            {
                return NotFound();
            }
            return View(Categoria);

        }

        #region API

        [ActionName("ValidaNombre")]
        public async Task<IActionResult> ValidaNombre(string nombre, int id = 0)
        {
            if (id == 0)
            {
                return Json(new
                {
                    data = (await _unidadTrabajo.Categoria.ObtenerTodos())
                .Any(b => b.Nombre.Trim().ToLower() == nombre.Trim().ToLower())
                });
            }
            return Json(new
            {
                data = (await _unidadTrabajo.Categoria.ObtenerTodos())
            .Any(b => b.Nombre.Trim().ToLower() == nombre.Trim().ToLower() && b.Id != id)
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if (categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[Ds.Exitosa] = "Categoria creeada con exito";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[Ds.Exitosa] = "Categoria actualizada correctamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[Ds.Error] = "Error en la operacion con la categoria";
            return View(categoria);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaBD = await _unidadTrabajo.Categoria.Obtener(id);
            if (categoriaBD == null)
            {
                return Json(new { success = false, message = "Error al obtener la categoria" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaBD);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria Eliminada correctamevte" });
        }

        #endregion
    }
}
