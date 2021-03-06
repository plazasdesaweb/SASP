﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Suma2Lealtad.Models.ViewModels.Home
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name="Usuario")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }
}