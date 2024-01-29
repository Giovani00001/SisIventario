using Microsoft.EntityFrameworkCore;
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
    internal class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext _context;
        public MarcaRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Actualizar(Marca marca)
        {
            var MarcaBD = _context.Marcas.FirstOrDefault(b => b.Id == marca.Id);
            if (MarcaBD != null)
            {
                MarcaBD.Nombre = marca.Nombre;
                MarcaBD.Descripcion = marca.Descripcion;
                MarcaBD.Estado = marca.Estado;
                _context.SaveChanges();
            }
        }
    }
}
