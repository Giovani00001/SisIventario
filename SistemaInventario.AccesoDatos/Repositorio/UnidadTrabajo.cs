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
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _context;
        public IBodegaRepositorio Bodega { get; private set; }
        public ICategoriaRepositorio Categoria { get; private set; }
        public IMarcaRepositorio Marca { get; private set; }
        public IProductoRepositorio Producto { get; private set; }
        public IUsuarioAplicacionRepositorio UsuarioAplicacion { get; private set; }
        public IBodegaProductoRepositorio BodegaProducto { get; private set; }
        public IInventarioRepositorio Inventario { get; private set; }
        public IInventarioDetalleRepositorio InventarioDetalle { get; private set; }
        public IKardexInventarioRepositorio KardexInventario { get; private set; }
        public UnidadTrabajo(ApplicationDbContext db)
        {
            _context = db;
            Bodega = new BodegaRepositorio(_context);
            Categoria = new CategoriaRepositorio(_context);
            Marca = new MarcaRepositorio(_context);
            Producto = new ProductoRepositorio(_context);
            UsuarioAplicacion = new UsuarioAplicacionRepositorio(_context);
            BodegaProducto = new BodegaProductoRepositorio(_context);
            Inventario = new InventarioRepositorio(_context);
            InventarioDetalle = new InventarioDetalleRepositorio(_context);
            KardexInventario = new KardexInventarioRepositorio(_context);

        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task Guardar()
        {
            await _context.SaveChangesAsync();
        }
    }
}
