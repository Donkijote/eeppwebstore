using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class Country
    {
        public string iso { get; set; }
        public string strName { get; set; }
        public string strPrintableName { get; set; }
        public string iso3 { get; set; }
        public short intNumCode { get; set; }
    }
}