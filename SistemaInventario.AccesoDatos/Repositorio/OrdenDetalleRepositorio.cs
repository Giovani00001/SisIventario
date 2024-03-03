using SistemaInventario.AccesoDatos.Data;
using SistemaInventario.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventario.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.AccesoDatos.Repositorio
{
    internal class OrdenDetalleRepositorio : Repositorio<OrdenDetalle>, IOrdenDetalleRepositorio
    {
        private readonly ApplicationDbContext _context;

        public OrdenDetalleRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public void Actualizar(OrdenDetalle ordenDetalle)
        {
            _context.Update(ordenDetalle);
        }
    }
}
