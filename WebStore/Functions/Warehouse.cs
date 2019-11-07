using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebStore.Models;

namespace WebStore.Functions
{
    public class Warehouse
    {
        public static List<Stock> GetStock(string cod)
        {
            using (ElectropEntities db = new ElectropEntities())
            {
                DateTime today = DateTime.Today;

                var x = from a in db.iw_gmovi
                        where (a.TipoOrigen == "D" || a.TipoDestino == "D") && (a.CodBode == "1" || a.CodBode == "30" || a.CodBode == "50") && a.Fecha <= today && a.Actualizado == -1 && a.CodProd == cod
                        select new
                        {
                            cod = a.CodProd,
                            codBode = a.CodBode,
                            cIn = a.CantIngresada,
                            cOut = a.CantDespachada
                        };
                var z = x.ToList();

                var query = z.GroupBy(j => new { j.cod, j.codBode })
                             .Select(i => new Stock
                             {
                                 cod = i.Key.cod,
                                 codBode = i.Key.codBode,
                                 cIn = i.Sum(j => Math.Round(Convert.ToDecimal(j.cIn), 2)),
                                 cOut = i.Sum(j => Math.Round(Convert.ToDecimal(j.cOut), 2)),
                                 TotalStock = i.Sum(j => Convert.ToInt32(j.cIn)) - i.Sum(j => Convert.ToInt32(j.cOut))
                             });
                return query.ToList();
            }
            
        }
    }
}