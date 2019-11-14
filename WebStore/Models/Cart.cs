using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class CartProductList
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int PriceInt { get; set; }
        public string PriceOff { get; set; }
        public int PriceOffInt { get; set; }
        public string PercentageOff { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
        public int SubtotalInt { get; set; }
        public int TotalInt { get; set; }
        public string SubtotalStr { get; set; }
        public string TotalStr { get; set; }
    }
}