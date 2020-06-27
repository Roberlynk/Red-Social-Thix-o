using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thix_o.ViewModels
{
    public class RegisterViewModel
    {
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Correo Electronico")]
        [DataType(DataType.EmailAddress)]
        public string Correo { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Usuario")]
        public string NombreUsuario { get; set; }
        [Required(ErrorMessage = "Este campo es requerido")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }
        [Display(Name = "Confimar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Contraseña", ErrorMessage = "Las Contraseñas no coinciden")]
        public string ConfimarContraseña { get; set; }
        public bool Statuss { get; set; }
    }
}
