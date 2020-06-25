using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thix_o.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Este campo es requerido")]
        [Display(Name ="Usuario")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
    }
}
