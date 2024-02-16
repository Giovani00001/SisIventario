using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using SistemaInventario.Utilidades;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Ds.Role_Admin)]
    public class BodegaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public BodegaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Bodega bodega = new();
            if (id == null)
            {
                //crear nueva bodega
                bodega.Estado = true;
                return View(bodega);
            }

            //actualiza bodega
            bodega = await _unidadTrabajo.Bodega.Obtener(id.GetValueOrDefault());
            if (bodega == null)
            {
                return NotFound();
            }
            return View(bodega);
        }

        #region API

        [ActionName("ValidarNombre")]
        public async Task<ActionResult> ValidarNombre(string nombree, int id = 0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Bodega.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombree.ToLower().Trim());
            }
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombree.ToLower().Trim() && b.Id != id);
            }

            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Bodega.ObtenerTodos();
            return Json(new { data = todos });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upsert(Bodega bodega)
        {
            if (ModelState.IsValid)
            {
                if (bodega.Id == 0)
                {
                    await _unidadTrabajo.Bodega.Agregar(bodega);
                    TempData[Ds.Exitosa] = "Bodega creada Exitosamente";
                }
                else
                {
                    _unidadTrabajo.Bodega.Actualizar(bodega);
                    TempData[Ds.Exitosa] = "Bodega Actualizada Exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[Ds.Error] = "Error en la operacion";
            return View(bodega);

        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var bodegaDb = await _unidadTrabajo.Bodega.Obtener(id);
            if (bodegaDb == null)
            {
                return Json(new { success = false, message = "Error al obtener la bodega" });

            }
            _unidadTrabajo.Bodega.Remover(bodegaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = $"Eliminada la bodega: {bodegaDb.Nombre}" });
        }


        #endregion
    }
}
