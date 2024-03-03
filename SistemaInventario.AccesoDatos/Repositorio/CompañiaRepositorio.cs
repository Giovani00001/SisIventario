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
    internal class CompañiaRepositorio : Repositorio<Compañia>, ICompañiaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CompañiaRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public void Actualizar(Compañia compañia)
        {
            var companiaBD = _context.Compañias.FirstOrDefault(b => b.Id == compañia.Id);
            if (companiaBD != null)
            {
                companiaBD.Nombre = compañia.Nombre;
                companiaBD.Descripcion = compañia.Descripcion;
                companiaBD.Ciudad = compañia.Ciudad;
                companiaBD.BodegaVentaId = compañia.BodegaVentaId;
                companiaBD.Telefono = compañia.Telefono;
                companiaBD.Pais = compañia.Pais;
                companiaBD.Direccion = compañia.Direccion;
                companiaBD.ActualizadoPorId = compañia.ActualizadoPorId;
                companiaBD.FechaActualizacion = compañia.FechaActualizacion;





                _context.SaveChanges();
            }
        }
    }
}
