using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace WebStore.Models
{
    public class Addresses
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int RegionId { get; set; }
        public string Region { get; set; }
        public int ProvinceId { get; set; }
        public string Province { get; set; }
        public int ComunaId { get; set; }
        public string Comuna { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public int? Poste { get; set; }
        public string Type { get; set; }
    }

    public class UserInfoUpdate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números")]
        public int UserPhone { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int Region { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int Province { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public int Comuna { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string City { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números")]
        public int PosteCode { get; set; }
        public bool Default { get; set; }
    }

    public class ChangePassword
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "El código no puede ser menor o mayor a 6 dígitos.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números son aceptados.")]
        public int ValidationCode { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "La clave debe ser de 8 caracteres y al menos contener 3 de 4 de: mayúscula (A-Z), minúscula (a-z), número (0-9) y caracter especial (e.g. !@#$%^&*)(opcional)")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Este campo es requerido.")]
        [StringLength(30, MinimumLength = 8, ErrorMessage = "Debe tener mínimo 8 caracteres.")]
        [EqualTo("PassRecovery", ErrorMessage = "Las contraseñas deben coincidir.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class BindingAddress
    {
        public Addresses Addresses { get; set; }
        public tblUsers User { get; set; }
        public UserInfoUpdate UserInfo { get; set; }
        public IEnumerable<tblRegiones> Regiones { get; set; }
        public IEnumerable<tblProvincias> Provinces { get; set; }
        public IEnumerable<tblComunas> Comunes { get; set; }
    }
}