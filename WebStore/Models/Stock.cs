using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Stock
    {
        public string cod { get; set; }
        public string codBode { get; set; }
        public decimal cIn { get; set; }
        public decimal cOut { get; set; }
        public int TotalStock { get; set; }
    }
}