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
    internal class InventarioDetalleRepositorio : Repositorio<InventarioDetalle>, IInventarioDetalleRepositorio
    {
        private readonly ApplicationDbContext _context;
        public InventarioDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Actualizar(InventarioDetalle inventarioDetalle)
        {
            var InventarioDetalleBD = _context.InventarioDetalles.FirstOrDefault(b => b.Id == inventarioDetalle.Id);
            if (InventarioDetalleBD != null)
            {
                InventarioDetalleBD.StockAnterior = inventarioDetalle.StockAnterior;
                InventarioDetalleBD.Cantidad = inventarioDetalle.Cantidad;
                _context.SaveChanges();
            }
        }


    }
}
