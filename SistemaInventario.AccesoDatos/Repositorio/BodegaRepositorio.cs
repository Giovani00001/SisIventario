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
    internal class BodegaRepositorio : Repositorio<Bodega>, IBodegaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public BodegaRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public void Actualizar(Bodega bodega)
        {
            var bodegaBD = _context.Bodegas.FirstOrDefault(b => b.Id == bodega.Id);
            if (bodegaBD != null)
            {
                bodegaBD.Nombre = bodega.Nombre;
                bodegaBD.Descripcion = bodega.Descripcion;
                bodega.Estado = bodega.Estado;
                _context.SaveChanges();
            }
        }
    }
}
