using System;
using System.Collections.Generic;

namespace Database.Model
{
    public partial class Usuario
    {
        public Usuario()
        {
            Amigo = new HashSet<Amigo>();
            Comentario = new HashSet<Comentario>();
        }

        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public bool Statuss { get; set; }

        public virtual ICollection<Amigo> Amigo { get; set; }
        public virtual ICollection<Comentario> Comentario { get; set; }
    }
}
