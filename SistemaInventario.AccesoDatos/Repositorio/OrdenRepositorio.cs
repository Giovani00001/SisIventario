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
    internal class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
    {
        private readonly ApplicationDbContext _context;

        public OrdenRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public void Actualizar(Orden orden)
        {
            _context.Update(orden);
        }

        public void ActualizarEstado(int id, string ordenEstado, string pagoEstado)
        {
            var ordenDb = _context.Orden.FirstOrDefault(o => o.Id == id);
            if (ordenDb != null)
            {
                ordenDb.EstadoOrden = ordenEstado;
                ordenDb.EstadoPago = pagoEstado;
            }


        }

        public void ActualizarPagoStripeId(int id, string sessionId, string transaccionId)
        {
            var ordenDb = _context.Orden.FirstOrDefault(o => o.Id == id);
            if (ordenDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    ordenDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(transaccionId))
                {
                    ordenDb.TransaccionId = transaccionId;
                    ordenDb.FechaPago = DateTime.Now;
                }
            }

        }
    }
}
