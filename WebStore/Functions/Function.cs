using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using WebStore.Models;

namespace WebStore.Functions
{
    public class Function
    {
        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }

        public string FormatNumber(int number)
        {
            return number.ToString("N0", CultureInfo.GetCultureInfo("es-CL"));
        }

        public string getTitle(string id)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var f = db.tblFamily.Where(a => a.strSeo == id)
                        .Select(x => x)
                        .FirstOrDefault();
                if (f == null)
                {
                    var c = db.tblCategories.Where(a => a.strSeo == id)
                            .Select(x => x)
                            .FirstOrDefault();
                    if (c == null)
                    {
                        return "Error";
                    }
                    else
                    {
                        return c.strNombre;
                    }
                }
                else
                {
                    return f.strName;
                }
            }
        }
    }
}