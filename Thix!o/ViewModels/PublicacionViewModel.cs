using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thix_o.ViewModels
{
    public class PublicacionViewModel
    {
        public int IdPublicacion { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Contenido")]
        public string Contenido { get; set; }
        public DateTime? FechaHora { get; set; }

        public int? IdUsuario { get; set; }
    }
}
