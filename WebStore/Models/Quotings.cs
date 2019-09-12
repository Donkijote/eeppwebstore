using System;
using System.Collections.Generic;
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
    }
}