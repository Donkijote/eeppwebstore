using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class QuotingsProductList
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string PriceOff { get; set; }
        public string PercentageOff { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public bool Stock { get; set; }
    }

    public class QuotingForm
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string QuotingName { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        public string QuotingLastname { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress(ErrorMessage = "Debe ingresar un email válido")]
        public string QuotingEmail { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Solo números son aceptados.")]
        public int QuotingPhone { get; set; }
        public string QuotingComment { get; set; }
    }

    public class QuotingModelBag
    {
        public List<QuotingsProductList> ProductList { get; set; }
        public QuotingForm Form { get; set; }
        public QuotingForm Values { get; set; }
    }

}