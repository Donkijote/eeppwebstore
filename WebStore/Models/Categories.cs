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
    }

    public class LeftMenu
    {
        public IEnumerable<tblFamily> Family { get; set; }
        public IEnumerable<tblCategories> Category { get; set; }
        public IEnumerable<TotalProductByFamily> FamilyTotal { get; set; }
        public IEnumerable<TotalProductByCategory> CategoryTotal { get; set; }
        public IEnumerable<TotalProductByBrand> BrandTotal { get; set; }
        public List<RangePrice> PriceRange { get; set; }
        public List<PriceRange> BetweenPrices { get; set; }     
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

    public class TotalProductByBrand
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int TotalProducts { get; set; }
    }

    public class RangePrice
    {
        public string minPrice { get; set; }
        public string maxPrice { get; set; }
    }

    public class PriceRange
    {
        public int lower { get; set; }
        public int low { get; set; }
        public int middle { get; set; }
        public int high { get; set; }
        public int higher { get; set; }
        public int highest { get; set; }
        public int top { get; set; }
    }
}