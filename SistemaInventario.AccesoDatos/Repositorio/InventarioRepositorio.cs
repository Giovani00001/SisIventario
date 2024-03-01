using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    internal class InventarioRepositorio : Repositorio<Inventario>, IInventarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        public InventarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Actualizar(Inventario inventario)
        {
            var InventarioBD = _context.Inventarios.FirstOrDefault(b => b.Id == inventario.Id);
            if (InventarioBD != null)
            {
                InventarioBD.BodegaId = inventario.BodegaId;
                InventarioBD.FechaFinal = inventario.FechaFinal;
                InventarioBD.Estado = inventario.Estado;

                _context.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenertodosDropDownList(string obj)
        {
            if (obj == "Bodega")
            {
                return _context.Bodegas.Where(b => b.Estado == true).Select(b => new SelectListItem
                {
                    Text = b.Nombre,
                    Value = b.Id.ToString()
                });
            }
            return null;
        }
    }
}
