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
    internal class KardexInventarioRepositorio : Repositorio<KardexInventario>, IKardexInventarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        public KardexInventarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public async Task RegitrarKardex(int bodegaProductoId, string tipo, string detalle, int stockAnterior, int cantidad, string usuarioId)
        {
            var bodegaProducto = await _context.BodegasProductos.Include(b => b.Producto).FirstOrDefaultAsync(b => b.Id == bodegaProductoId);
            if (tipo == "Entrada")
            {
                KardexInventario kardex = new KardexInventario();
                kardex.Tipo = tipo;
                kardex.BodegaProductoId = bodegaProductoId;
                kardex.Detalle = detalle;
                kardex.StockAnterior = stockAnterior;
                kardex.Cantidad = cantidad;
                kardex.Costo= bodegaProducto.Producto.Costo;
                kardex.Stock = stockAnterior + cantidad;
                kardex.Total = kardex.Stock * kardex.Costo;
                kardex.usuarioAplicacionId = usuarioId;
                kardex.FechaRegistro = DateTime.Now;

                await _context.AddAsync(kardex);
                await _context.SaveChangesAsync();

            }
            if (tipo == "Salida")
            {
                KardexInventario kardex = new KardexInventario();
                kardex.Tipo = tipo;
                kardex.BodegaProductoId = bodegaProductoId;
                kardex.Detalle = detalle;
                kardex.StockAnterior = stockAnterior;
                kardex.Cantidad = cantidad;
                kardex.Costo = bodegaProducto.Producto.Costo;
                kardex.Stock = stockAnterior - cantidad;
                kardex.Total = kardex.Stock * kardex.Costo;
                kardex.usuarioAplicacionId = usuarioId;
                kardex.FechaRegistro = DateTime.Now;

                await _context.AddAsync(kardex);
                await _context.SaveChangesAsync();

            }
        }
    }
}
