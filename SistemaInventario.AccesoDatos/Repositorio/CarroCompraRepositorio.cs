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
    internal class CarroCompraRepositorio : Repositorio<CarroCompra>, ICarroCompraRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CarroCompraRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public void Actualizar(CarroCompra carroCompra)
        {
            _context.Update(carroCompra);
        }
    }
}
