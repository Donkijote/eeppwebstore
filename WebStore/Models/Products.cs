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
        public int refCategoria { get; set; }
        public string categoryName { get; set; }
    }
}