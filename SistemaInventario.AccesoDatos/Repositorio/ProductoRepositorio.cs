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
    internal class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly ApplicationDbContext _context;
        public ProductoRepositorio(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }

        public void Actualizar(Producto producto)
        {
            var ProductoBD = _context.Productos.FirstOrDefault(b => b.Id == producto.Id);
            if (ProductoBD != null)
            {
                if (producto.ImagenUrl != null)
                {
                    ProductoBD.ImagenUrl = producto.ImagenUrl;
                }

                ProductoBD.NumSerie = producto.NumSerie;
                ProductoBD.Descripcion = producto.Descripcion;
                ProductoBD.Precio = producto.Precio;
                ProductoBD.Costo = producto.Costo;
                ProductoBD.CategoriaId = producto.CategoriaId;
                ProductoBD.MarcaId = producto.MarcaId;
                ProductoBD.PadreId = producto.PadreId;
                ProductoBD.Estado = producto.Estado;


                _context.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj)
        {
            if (obj == "Categoria")
            {
                return _context.Categorias.Where(b => b.Estado == true).Select(b => new SelectListItem
                {
                    Text = b.Nombre,
                    Value = b.Id.ToString()
                });
            }
            if (obj == "Marca")
            {
                return _context.Marcas.Where(b => b.Estado == true).Select(b => new SelectListItem
                {
                    Text = b.Nombre,
                    Value = b.Id.ToString()
                });
            }
            if (obj == "Producto")
            {
                return _context.Productos.Where(b => b.Estado == true).Select(b => new SelectListItem
                {
                    Text = b.Descripcion,
                    Value = b.Id.ToString()
                });
            }
            else
            {
                throw new Exception("No existe la opcion");
            }

        }
    }
}
