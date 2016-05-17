using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models.ViewModels.Home
{
    public class CambiarPasswordViewModel
    {
        [Required]
        [Display(Name="Usuario : ")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Nueva Contraseña : ")]
        public string NewPassword1 { get; set; }

        [Required]
        [Display(Name = "Confirmación Nueva Contraseña : ")]
        public string NewPassword2 { get; set; }
    }
}