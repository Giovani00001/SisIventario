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
    internal class BodegaProductoRepositorio : Repositorio<BodegaProducto>, IBodegaProductoRepositorio
    {
        private readonly ApplicationDbContext _context;
        public BodegaProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Actualizar(BodegaProducto bodegaProducto)
        {
            var BodegaProductoBD = _context.BodegasProductos.FirstOrDefault(b => b.Id == bodegaProducto.Id);
            if (BodegaProductoBD != null)
            {
                BodegaProductoBD.Cantidad = bodegaProducto.Cantidad;


                _context.SaveChanges();
            }
        }

        
    }
}
