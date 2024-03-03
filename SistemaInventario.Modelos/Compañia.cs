using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Compañia
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(60, ErrorMessage = "Nombre debe contener maximo 60 caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Descripcion es requerido")]
        [MaxLength(100, ErrorMessage = "Descripcion debe contener maximo 100 caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "Pais es requerido")]
        [MaxLength(80, ErrorMessage = "Pais debe contener maximo 60 caracteres")]
        public string Pais { get; set; }
        [Required(ErrorMessage = "Ciudad es requerido")]
        [MaxLength(80, ErrorMessage = "Ciudad debe contener maximo 60 caracteres")]
        public string Ciudad { get; set; }
        [Required(ErrorMessage = "Direccion es requerido")]
        [MaxLength(100, ErrorMessage = "Direccion debe contener maximo 60 caracteres")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "Telefono es requerido")]
        [MaxLength(100, ErrorMessage = "Telefono debe contener maximo 60 caracteres")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "Bodega de Venta es requerido")]
        public int BodegaVentaId { get; set; }
        [ForeignKey("BodegaVentaId")]
        public Bodega Bodega { get; set; }

        public string CreadoPorId { get; set; }
        [ForeignKey("CreadoPorId")]
        public UsuarioAplicacion CreadoPor { get; set; }

        public string ActualizadoPorId { get; set; }
        [ForeignKey("ActualizadoPorId")]
        public UsuarioAplicacion ActualizadoPor { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion{ get; set; }


    }
}
