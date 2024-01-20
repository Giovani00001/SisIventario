using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventario.Modelos
{
    public class Bodega
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(60, ErrorMessage = "El nombre debe ser maximo de 60 caracteres")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Descripciones requerido")]
        [MaxLength(100, ErrorMessage = "Descripcion debe ser maximo de 100 caracteres")]
        public string Descripcion { get; set; }
        [Required]
        public bool Estado { get; set; }
    }
}
