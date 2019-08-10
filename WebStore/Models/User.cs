using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class LogInRegistration
    {
        public Registration Registration { get; set; }
        public LogIn LogIn { get; set; }
    }
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
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "La clave debe ser de 8 caracteres y al menos contener 3 de 4 de: mayúscula (A-Z), minúscula (a-z), número (0-9) y caracter especial (e.g. !@#$%^&*)(opcional)")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage ="Debe tener mínimo 8 caracteres.")]
        [Compare("Pass", ErrorMessage = "Las contraseñas deben coincidir.")]
        [DataType(DataType.Password)]
        public string Passre { get; set; }
    }
    public class LogIn
    {
        [Required(ErrorMessage = "Este campo es requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar Email válido.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RecoveryPassword
    {
        [Required(ErrorMessage = "Campo Requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido.")]
        public string EmailRecovery { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código no puede ser menor a 6 dígitos.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números son aceptados.")]
        public string CodRecovery { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "La clave debe ser de 8 caracteres y al menos contener 3 de 4 de: mayúscula (A-Z), minúscula (a-z), número (0-9) y caracter especial (e.g. !@#$%^&*)(opcional)")]
        [DataType(DataType.Password)]
        public string PassRecovery { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [Compare("PassRecovery", ErrorMessage = "Las contraseñas deben coincidir.")]
        [DataType(DataType.Password)]
        public string PassRecoveryConfirmation { get; set; }
    }
}
