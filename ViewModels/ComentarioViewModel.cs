using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ComentarioViewModel
    {
        public int IdComentario { get; set; }
        public int? IdUsuario { get; set; }
        public int? IdPublicacion { get; set; }
        public string Contenido { get; set; }
        public DateTime? FechaHora { get; set; }
        public string NombreUsuario { get; set; }
    }
}
