using System;
using System.Collections.Generic;

namespace Database.Model
{
    public partial class Comentario
    {
        public int? IdComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdPublicacion { get; set; }

        public virtual Publicacion IdPublicacionNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
