using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Registration
    {
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar Email válido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Password)]
        [Compare("Pass", ErrorMessage = "Las contraseñas deben coincidir.")]
        public string Passre { get; set; }
    }
}