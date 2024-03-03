
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos.ViewModels;
using SistemaInventario.Utilidades;
using System.Security.Claims;

namespace SistemaInventario.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Ds.Role_Admin)]
    public class CompañiaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CompañiaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;

        }
        public async Task<IActionResult> Upsert()
        {
            CompañiaVM compañiaVM = new CompañiaVM()
            {
                Compañia = new Modelos.Compañia(),
                BodegaLista = _unidadTrabajo.Inventario.ObtenertodosDropDownList("Bodega")

            };

            compañiaVM.Compañia = await _unidadTrabajo.Compañia.ObtenerPrimero();

            if (compañiaVM.Compañia == null)
            {
                compañiaVM.Compañia = new Modelos.Compañia();
            }
            return View(compañiaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CompañiaVM compañiaVM)
        {
            if (ModelState.IsValid)
            {
                TempData[Ds.Exitosa] = "Compañia grabada exitosamente";
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (compañiaVM.Compañia.Id == 0)
                {
                    compañiaVM.Compañia.CreadoPorId = claim.Value;
                    compañiaVM.Compañia.ActualizadoPorId = claim.Value;
                    compañiaVM.Compañia.FechaActualizacion = DateTime.Now;
                    compañiaVM.Compañia.FechaCreacion = DateTime.Now;
                    await _unidadTrabajo.Compañia.Agregar(compañiaVM.Compañia);
                }
                else
                {
                    compañiaVM.Compañia.ActualizadoPorId = claim.Value;
                    compañiaVM.Compañia.FechaActualizacion = DateTime.Now;
                    _unidadTrabajo.Compañia.Actualizar(compañiaVM.Compañia);
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index", "Home", new { area = "Inventario" });
            }
            TempData[Ds.Error] = "Ocurrio un error al grabar compañia";
            return View(compañiaVM);
        }

    }

}
