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
    }

    public class Brands
    {
        public string StrName { get; set; }
        public string StrImg { get; set; }
    }
}