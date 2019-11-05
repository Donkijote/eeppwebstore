using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;
using WebStore.Routing;
using System.Threading;
using System.Globalization;

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
            var x = (from p in db.tblProducts
                     join f in db.tblFamily
                     on p.refFamily equals f.idFamily
                     join c in db.tblCategories
                     on p.refCategory equals c.idCategoria
                     where p.strCode == idp
                     select new
                     {
                         Id = p.idProduct,
                         Codigo = p.strCode,
                         CodigoS = p.strCodeS,
                         Name = p.strName,
                         Price = p.intPrice,
                         Category = c.strSeo,
                         Offert = p.refOffert,
                         OffertTime = p.refOfferTime
                     }).AsEnumerable()
                        .Select(p => new ProductsSingle
                        {
                            IdCode = p.Id,
                            strCodigo = p.Codigo,
                            Cod = p.CodigoS,
                            strNombre = p.Name,
                            intPrecio = Function.FormatNumber(p.Price),
                            intPrecioNum = p.Price,
                            categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(p.Category),
                            Offert = p.Offert,
                            OffertTime = p.OffertTime
                        }).FirstOrDefault();
            ViewBag.Category = id;

            var tList = db.tblOffertTime.Where(t => t.strTime >= DateTime.Now).Select(j => j).ToList();

            var oList = db.tblOffert.Select(r => r).ToList();

            if (oList.Any())
            {
                if (x.Offert != null && x.Offert > 0)
                {
                    int percet = (int)oList.Where(o => o.idOffert == x.Offert).Select(r => r.intPercentage).FirstOrDefault();
                    x.intPercent = percet + "%";
                    x.intPrecioOff = Function.FormatNumber(x.intPrecioNum - (x.intPrecioNum * percet / 100));
                }
            }

            if (tList.Any())
            {
                if (x.OffertTime != null && x.OffertTime > 0)
                {
                    var first = (from t in tList
                                    where t.strTime > DateTime.Now
                                    orderby t.strTime ascending
                                    select t.strTime).First();
                    TimeSpan timeDiff = first - DateTime.Now;
                    int percentOff = tList.Where(t => t.idOffertTime == x.OffertTime && t.strTime == first).Select(t => (int)t.intPercentageTime).FirstOrDefault();
                    if (tList.Any(t => t.idOffertTime == x.OffertTime && t.strTime == first))
                    {
                        x.TimeOffer = true;
                        x.intPrecentOff = percentOff + "%";
                        x.intPrecioOff = Function.FormatNumber((int)Decimal.Parse(x.intPrecio) - (int)(Decimal.Parse(x.intPrecio) * percentOff / 100));
                        x.Time = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds);
                    }
                }
            }

            ViewBag.Title = Resources.Titles.Product + " " + x.strNombre;
            ViewBag.Breadcrumbs = Resources.Titles.Product + " #" + x.strCodigo;

            x.Stock = stock.GetStock(x.Cod);

            x.Ficha = db.tblFicha.Where(w => w.refCodProd == x.strCodigo).FirstOrDefault();

            if(Session["id"] != null)
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                var History = db.tblHistory.Where(h => h.refUser == UserId).FirstOrDefault();
                if(History != null)
                {
                    var HistoryDet = (from a in db.tblHistoryDet
                                      where a.refHistory == History.IdHistory && a.refProduct == x.IdCode
                                      select new { 
                                        Codigo = a.refProduct
                                      }).FirstOrDefault();

                    if(HistoryDet == null)
                    {
                        var newHistoryDet = new tblHistoryDet()
                        {
                            refHistory = History.IdHistory,
                            refProduct = x.IdCode
                        };
                        db.tblHistoryDet.Add(newHistoryDet);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var newHistory = new tblHistory()
                    {
                        refUser = UserId
                    };
                    db.tblHistory.Add(newHistory);
                    db.SaveChanges();
                    var newHistoryDet = new tblHistoryDet
                    {
                        refHistory = newHistory.IdHistory,
                        refProduct = x.IdCode
                    };
                    db.tblHistoryDet.Add(newHistoryDet);
                    db.SaveChanges();
                }
            }
            else
            {
                if (Request.Cookies["History"] != null)
                {
                    var History = Request.Cookies["History"];
                    var items = History.Values.AllKeys.SelectMany(History.Values.GetValues, (k, v) => new { key = k, value = v });
                    string newName = "Object-" + (Int32.Parse(items.Last().key.ToString().Split('-')[1]) + 1);

                    History.Expires = DateTime.Now.AddHours(24);

                    if(!items.Any(a => a.value == x.strCodigo))
                    {
                        History[newName] = x.strCodigo;
                        Response.Cookies.Add(History);
                    }
                }
                else
                {
                    HttpCookie History = new HttpCookie("History");
                    History.Expires = DateTime.Now.AddHours(24);

                    History["Object-1"] = x.strCodigo;

                    Response.Cookies.Add(History);
                }
            }

            if(Session["Question"] != null && Session["Question"].ToString() != "")
            {
                string addQuestion = AddQuestionByRedirecction(x.strCodigo);
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
            string code = x.strCodigo;
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