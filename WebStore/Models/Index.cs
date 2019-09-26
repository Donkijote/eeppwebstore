using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;

namespace WebStore.Models
{
    public class Index
    {
        public BindingCateogyFamilyChild Binding { get; set; }
        public IEnumerable<Brands> Brands { get; set; }
        public IEnumerable<Products> ProductTime { get; set; }
        public IEnumerable<Products> ProductsList { get; set; }
        public IEnumerable<Products> ProductsOffer { get; set; }
        public List<Products> FirstHistory { get; set; }
        public List<Products> SecondHistory { get; set; }
        public List<Products> ThirdHistory { get; set; }
    }

    public class Brands
    {
        public string StrName { get; set; }
        public string StrImg { get; set; }
    }
}