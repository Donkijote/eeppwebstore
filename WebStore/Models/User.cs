using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Models
{
    public class LogInRegistration
    {
        public Registration Registration { get; set; }
        public LogIn LogIn { get; set; }
    }
    public class Registration
    {
        public int RegistrationType { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression("[0-9]{1,2}.[0-9]{3}.[0-9]{3}-[0-9Kk]{1}", ErrorMessage = "RUT inválido. Favor seguir Ej: xx.xxx.xxx-x")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "RUT debe tener 12 dígitos.")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números.")]
        public int Phone { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int States { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int Provinces { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int Comunes { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string City { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string AddressOne { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string Type { get; set; }
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

        public CompanyRegistration Company { get; set; }
    }
    public class CompanyRegistration
    {
        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression("[0-9]{1,2}.[0-9]{3}.[0-9]{3}-[0-9Kk]{1}", ErrorMessage = "RUT inválido. Favor seguir Ej: xx.xxx.xxx-x")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "RUT debe tener 12 dígitos.")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string CompanyActivity { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números.")]
        public int CompanyPhone { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int CompanyStates { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int CompanyProvinces { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public int CompanyComunes { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string CompanyCity { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        public string CompanyAddressOne { get; set; }
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
        public string CartString { get; set; }
        public string HistoryString { get; set; }
    }
    public class ExternalLogin
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
        public string CartString { get; set; }
        public string HistoryString { get; set; }
    }
    public class RecoveryPassword
    {
        [Required(ErrorMessage = "Campo Requerido.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido.")]
        public string EmailRecovery { get; set; }

        [Required(ErrorMessage = "Campo Requerido.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código no puede ser menor o mayor a 6 dígitos.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números son aceptados.")]
        public string CodRecovery { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "La clave debe ser de 8 caracteres y al menos contener 3 de 4 de: mayúscula (A-Z), minúscula (a-z), número (0-9) y caracter especial (e.g. !@#$%^&*)(opcional)")]
        [DataType(DataType.Password)]
        public string PassRecovery { get; set; }

        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [EqualTo("PassRecovery", ErrorMessage = "Las contraseñas deben coincidir.")]
        [DataType(DataType.Password)]
        public string PassRecoveryConfirmation { get; set; }
    }
    public class Avatars
    {
        public string Icon { get; set; }
        public bool Selected { get; set; }
    }
}
