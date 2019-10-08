using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Functions;

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
        public int intPrecioNum { get; set; }
        public string categoryName { get; set; }
        public string categorySeo { get; set; }
        public string productSeo { get; set; }
        public string Brand { get; set; }
        public bool TimeOffer { get; set; }
        public string Time { get; set; }
        public int? Offert { get; set; }
        public int? OffertTime { get; set; }
    }

    public class ProductsSingle
    {
        public string Cod { get; set; }
        public string strNombre { get; set; }
        public string strCodigo { get; set; }
        public string intPrecio { get; set; }
        public string intPercent { get; set; }
        public string intPrecioOff { get; set; }
        public string intPrecentOff { get; set; }
        public int refCategoria { get; set; }
        public int intPrecioNum { get; set; }
        public int intPrecioOffNum { get; set; }
        public string categoryName { get; set; }
        public string categorySeo { get; set; }
        public string productSeo { get; set; }
        public bool TimeOffer { get; set; }
        public string Time { get; set; }
        public int? Offert { get; set; }
        public int? OffertTime { get; set; }
        public tblFicha Ficha { get; set; }
        public IEnumerable<Stock> Stock { get; set; }
    }

    public class BindProductPageModels
    {
        public List<ProductsSingle> Products { get; set; }
        public List<Questions> UserQuestions { get; set; }
        public List<Questions> OthersQuestions { get; set; }
    }
}