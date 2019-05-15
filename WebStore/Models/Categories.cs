using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Categories
    {
        public string strNombre { get; set; }

        public bool intActivo { get; set; }
    }

    public class BindingCateogyFamilyChild
    {
        public IEnumerable<tblFamily> family { get; set; }
        public IEnumerable<tblCategories> category { get; set; }
        public IEnumerable<TotalProductByFamily> familyTotal { get; set; }
        public IEnumerable<TotalProductByCategory> categoryTotal { get; set; }
    }

    public class TotalProductByCategory
    {
        public int CategoryId { get; set; }
        public int TotalProducts { get; set; }
    }

    public class TotalProductByFamily
    {
        public int FamilyId { get; set; }
        public int TotalProducts { get; set; }
    }
}