using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Checkout
    {
        public BindSelect BindSelect { get; set; }
        public List<CartProductList> ProductList { get; set; }
        public tblUsers User { get; set; }
        public tblAddresses Address { get; set; }
    }
}