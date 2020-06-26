using System;
using System.Collections.Generic;

namespace Database.Model
{
    public partial class Publicacion
    {
        public Publicacion()
        {
            Comentario = new HashSet<Comentario>();
        }

        public int IdPublicacion { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaHora { get; set; }
        public int? IdUsuario { get; set; }

        public virtual ICollection<Comentario> Comentario { get; set; }
    }
}
