using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Products
    {
        public string strNombre { get; set; }
        public string strCodigo { get; set; }
        public string intPrecio { get; set; }
        public string intPercent { get; set; }
        public string intPrecioOff { get; set; }
        public string intPrecentOff { get; set; }
        public int refCategoria { get; set; }
        public string categoryName { get; set; }
        public string categorySeo { get; set; }
        public string productSeo { get; set; }
        public bool TimeOffer { get; set; }
        public string Time { get; set; }
    }
}