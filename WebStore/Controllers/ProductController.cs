using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;
using WebStore.Routing;
using System.Threading;

namespace WebStore.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Category(string id, string idp)
        {
            webstoreEntities db = new webstoreEntities();
            ElectropEntities dbE = new ElectropEntities();
            Warehouse stock = new Warehouse();
            BindProductPageModels models = new BindProductPageModels();
            var o = dbE.iw_tlprprod.Where(i => i.CodLista == "16").ToList();
            var x = (from a in dbE.iw_tprod
                     join b in dbE.iw_tlprprod
                     on a.CodProd equals b.CodProd
                     join c in dbE.iw_tlispre
                     on b.CodLista equals c.CodLista
                     where a.CodBarra == idp && c.CodLista == "15"
                     select new
                     {
                         strNombre = a.DesProd,
                         strCodigo = a.CodProd,
                         strCod = a.CodBarra,
                         intPrecio = a.PrecioVta,
                         intPercent = b.ValorPct
                     })
                     .AsEnumerable()
                     .Select(p => new ProductsSingle
                     {
                         Cod = p.strCodigo,
                         strCodigo = p.strCod,
                         strNombre = p.strNombre.ToLower(),
                         intPrecio = Function.FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100)))),
                         intPrecioNum = (int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100))),
                         intPrecentOff = o.Any(l => l.CodProd == p.strCodigo) ? o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() + "%" : "0",
                         intPrecioOff = o.Any(l => l.CodProd == p.strCodigo) ? Function.FormatNumber((int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100))) - (int)(((p.intPrecio + (p.intPrecio * (p.intPercent / 100))) * o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100))) : "0",
                         intPrecioOffNum = o.Any(l => l.CodProd == p.strCodigo) ? (int)(p.intPrecio + (p.intPrecio * (p.intPercent / 100))) - (int)(((p.intPrecio + (p.intPrecio * (p.intPercent / 100))) * o.Where(i => i.CodProd == p.strCodigo).Select(j => (int)j.ValorPct).FirstOrDefault() / 100)) : 0,
                         intPercent = p.intPercent + "%",
                         categorySeo = id
                     })
                     .ToList();
            ViewBag.Category = id;

            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();

            if (tList.Any())
            {
                foreach (var a in x)
                {
                    ViewBag.Title = Resources.Titles.Product + " " + a.strNombre;
                    ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + a.strCodigo;

                    var first = (from t in tList
                                 where t.strTime > DateTime.Now
                                 orderby t.strTime ascending
                                 select t.strTime).First();
                    ViewBag.first = first;
                    TimeSpan timeDiff = first - DateTime.Now;
                    int percentOff = tList.Where(t => t.refCodProd == a.strCodigo && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                    if (tList.Any(t => t.refCodProd == a.strCodigo && t.strTime == first))
                    {
                        a.TimeOffer = true;
                        a.intPrecentOff = percentOff + "%";
                        a.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(a.intPrecio) - (int)(Double.Parse(a.intPrecio) * percentOff / 100));
                        a.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                    }
                }
            }
            else
            {
                foreach (var a in x)
                {
                    ViewBag.Title = Resources.Titles.Product + " " + a.strNombre;
                    ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + a.strCodigo;
                }
            }

            var Ficha = db.tblFicha.Select(f => f);

            foreach (var p in x)
            {
                p.Stock = stock.GetStock(p.Cod);

                p.Ficha = db.tblFicha.Where(w => w.refCodProd == p.strCodigo).FirstOrDefault();

            }
            

            if(Session["id"] != null)
            {

            }
            else
            {
                if (Request.Cookies["History"] != null)
                {
                    var History = Request.Cookies["History"];
                    var items = History.Values.AllKeys.SelectMany(History.Values.GetValues, (k, v) => new { key = k, value = v });
                    string newName = "Object-" + (Int32.Parse(items.Last().key.ToString().Split('-')[1]) + 1);

                    History.Expires = DateTime.Now.AddHours(24);

                    foreach (var i in x)
                    {
                        if(!items.Any(a => a.value == i.Cod))
                        {
                            History[newName] = i.Cod;
                            Response.Cookies.Add(History);
                        }
                       /* History["strCodigo"] = i.strCodigo;
                        History["strNombre"] = i.strNombre;
                        History["intPrecio"] = i.intPrecio;
                        History["intPrecioOff"] = i.intPrecioOff;
                        History["categorySeo"] = i.categorySeo;
                        History["Date"] = $"{DateTime.Today}";*/
                    }
                }
                else
                {
                    HttpCookie History = new HttpCookie("History");
                    History.Expires = DateTime.Now.AddHours(24);

                    foreach (var i in x)
                    {
                        History["Object-1"] = i.Cod;
                        /* History["strCodigo"] = i.strCodigo;
                         History["strNombre"] = i.strNombre;
                         History["intPrecio"] = i.intPrecio;
                         History["intPrecioOff"] = i.intPrecioOff;
                         History["categorySeo"] = i.categorySeo;
                         History["Date"] = $"{DateTime.Today}";*/
                    }


                    Response.Cookies.Add(History);
                }
            }

            if(Session["Question"] != null && Session["Question"].ToString() != "")
            {
                string addQuestion = AddQuestionByRedirecction(x.FirstOrDefault().strCodigo);
                if(addQuestion == "OK")
                {
                    ViewBag.addQuestionFromRedirect = "OK";
                }
                else
                {
                    ViewBag.addQuestionFromRedirect = addQuestion;
                }
            }
            List<Questions> question = new List<Questions>();
            string code = x.FirstOrDefault().strCodigo;
            var questions = (from q in db.tblQuestions
                             where q.refProduct == code && q.intStatus == 2
                             select new
                             {
                                 Id = q.idQuestion,
                                 Question = q.strQuestion,
                                 QuestionDate = q.strDate,
                                 User = q.refUser
                             }).ToList();
            foreach(var i in questions)
            {
                Answers ans = (from a in db.tblAnswers
                               where a.refQuestion == i.Id
                               select new Answers
                               {
                                   Answer = a.strAnswer,
                                   AnswerDate = a.strDate
                               }).FirstOrDefault();
                question.Add(new Questions {
                    Question = i.Question,
                    QuestionDate = i.QuestionDate,
                    Answer = ans,
                    User = i.User
                });
            }

            
            if(Session["id"] != null)
            {
                int UserId = (int)Session["id"];
                models.UserQuestions = question.Where(q => q.User == UserId).ToList();
                models.OthersQuestions = question.Where(q => q.User != UserId).ToList();
            }
            else
            {
                models.OthersQuestions = question;
            }

            models.Products = x;

            return View("Product", models);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddQuestion(string Question, string Code)
        {
            if(Session["id"] != null)
            {
                string response = AddQuestionToDB(Question, Code);

                if(response == "OK")
                {
                    return Json(new { status = "OK", title = "Pregunta", responseText = "Su pregunta ha sido enviada", isLoggedIn = true });
                }
                else
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = response, isLoggedIn = true });
                }
            }
            else
            {
                Session["Question"] = Question;
                return Json(new { status = "OK", title = "Pregunta", responseText = "", isLoggedIn = false, redirectToAction = Url.Action("LogIn", "User", new { ReturnUrl = Request.UrlReferrer.AbsoluteUri }) });
            }
        }

        private string AddQuestionByRedirecction(string Code)
        {
            string resp = AddQuestionToDB(Session["Question"].ToString(), Code);
            Session.Remove("Question");
            return resp;
        }

        private string AddQuestionToDB(string Question, string Code)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                try
                {
                    tblQuestions question = new tblQuestions()
                    {
                        strQuestion = Question,
                        strDate = DateTime.Now,
                        refUser = (int)Session["id"],
                        refProduct = Code,
                        boolAnswered = false,
                        intStatus = 1
                    };

                    db.tblQuestions.Add(question);
                    db.SaveChanges();
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
    }
}