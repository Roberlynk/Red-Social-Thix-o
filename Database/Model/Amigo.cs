using System;
using System.Collections.Generic;

namespace Database.Model
{
    public partial class Amigo
    {
        public int IdAmigo { get; set; }
        public int IdUsuario { get; set; }

        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
