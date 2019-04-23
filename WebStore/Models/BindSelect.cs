using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebStore.Models
{
    public class BindSelect
    {
        public IEnumerable<tblRegiones> regions { get; set; }
        public IEnumerable<tblProvincias> provinces { get; set; }
        public IEnumerable<tblComunas> comunes { get; set; }
        public IEnumerable<tblCountry> countries { get; set; }
    }
}