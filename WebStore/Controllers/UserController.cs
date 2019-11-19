using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;
using System.Web.Security;
using System.Globalization;
using WebStore.Routing;

namespace WebStore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Cart()
        {
            if (Session["id"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];

                    List<tblCartQueDet> products = (from a in db.tblCartQue
                                                    join b in db.tblCartQueDet
                                                    on a.IdCartQue equals b.refCartQue
                                                    where a.refUser == UserId
                                                    select b).ToList();

                    List<CartProductList> productsToReturn = Function.GetCartProducts(products, db);


                    return View(productsToReturn);
                }
            }
            else
            {
                List<CartProductList> productLists = Session["CartList"] as List<CartProductList>;
                return View(productLists);
            }
        }
        [HttpPost]
        public JsonResult AddProductToCart(CartProductList CartProduct)
        {
            try
            {
                if(Session["id"] != null)
                {
                    using(webstoreEntities db = new webstoreEntities())
                    {
                        int UserId = (int)Session["id"];

                        tblCartQue cartQue = db.tblCartQue.Where(x => x.refUser == UserId).SingleOrDefault();

                        if(cartQue != null)
                        {
                            tblCartQueDet cartQueDet = db.tblCartQueDet.Where(x => x.refCartQue == cartQue.IdCartQue && x.refCodProd == CartProduct.Code).SingleOrDefault();

                            if(cartQueDet != null)
                            {
                                try
                                {
                                    cartQueDet.Quantity += CartProduct.Quantity;
                                    db.SaveChanges();
                                    return Json(new { status = "OK" });
                                }
                                catch(Exception ex)
                                {
                                    return Json(new { status = "error", title="Ups...!", responseText = $"Error al añadir producto: <br>{ex.ToString()}" });
                                }

                            }
                            else
                            {
                                tblCartQueDet newCartQueDet = new tblCartQueDet
                                {
                                    refCartQue = cartQue.IdCartQue,
                                    refCodProd = CartProduct.Code,
                                    Quantity = CartProduct.Quantity
                                };
                                try
                                {
                                    db.tblCartQueDet.Add(newCartQueDet);
                                    db.SaveChanges();
                                    return Json(new { status = "OK" });
                                }
                                catch(Exception ex)
                                {
                                    return Json(new { status = "error", title = "Ups...!", responseText = $"Error al añadir producto: <br>{ex.ToString()}" });
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                tblCartQue newCartQue = new tblCartQue
                                {
                                    refUser = UserId
                                };

                                db.tblCartQue.Add(newCartQue);

                                tblCartQueDet newCartQueDet = new tblCartQueDet
                                {
                                    refCartQue = newCartQue.IdCartQue,
                                    refCodProd = CartProduct.Code,
                                    Quantity = CartProduct.Quantity
                                };

                                db.tblCartQueDet.Add(newCartQueDet);
                                db.SaveChanges();
                                return Json(new { status = "OK" });
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = "error", title = "Ups...!", responseText = $"Error al añadir producto: <br>{ex.ToString()}" });
                            }
                        }
                    }
                }
                else
                {
                    List<CartProductList> productLists = Session["CartList"] as List<CartProductList>;


                    if (productLists != null)
                    {
                        if (productLists.Any(x => x.Code == CartProduct.Code))
                        {
                            foreach (var i in productLists)
                            {
                                if (i.Code == CartProduct.Code)
                                {
                                    i.Quantity += CartProduct.Quantity;
                                    i.SubtotalInt = i.PriceInt * i.Quantity;
                                    i.TotalInt = i.PriceOffInt > 0 ? i.PriceOffInt * i.Quantity : i.PriceInt * i.Quantity;
                                    i.SubtotalStr = Function.FormatNumber(i.SubtotalInt);
                                    i.TotalStr = Function.FormatNumber(i.TotalInt);
                                }
                            }
                        }
                        else
                        {
                            productLists.Add(CartProduct);
                        }
                        Session["CartList"] = productLists;
                        return Json(new { status = "OK" });
                    }
                    else
                    {
                        List<CartProductList> Products = new List<CartProductList>
                        {
                            new CartProductList
                            {
                                Code = CartProduct.Code,
                                Price = CartProduct.Price,
                                Name = CartProduct.Name,
                                PriceInt = CartProduct.PriceInt,
                                PercentageOff = CartProduct.PercentageOff,
                                PriceOff = CartProduct.PriceOff,
                                PriceOffInt = CartProduct.PriceOffInt,
                                Quantity = CartProduct.Quantity,
                                Category = CartProduct.Category,
                                SubtotalInt = CartProduct.PriceInt * CartProduct.Quantity,
                                TotalInt = CartProduct.PriceOffInt > 0 ? CartProduct.PriceOffInt * CartProduct.Quantity : CartProduct.PriceInt * CartProduct.Quantity,
                                SubtotalStr = Function.FormatNumber(CartProduct.PriceInt * CartProduct.Quantity),
                                TotalStr = Function.FormatNumber(CartProduct.PriceOffInt > 0 ? CartProduct.PriceOffInt * CartProduct.Quantity : CartProduct.PriceInt * CartProduct.Quantity)
                            }
                        };
                        Session["CartList"] = Products;
                        return Json(new { status = "OK" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", responseText = ex.ToString() });
            }
        }
        [HttpPost]
        public JsonResult RemoveItemFromCartList(string id)
        {
            if (Session["id"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];

                    tblCartQueDet cartQueDet = (from a in db.tblCartQue
                                                join b in db.tblCartQueDet
                                                on a.IdCartQue equals b.refCartQue
                                                where a.refUser == UserId && b.refCodProd == id
                                                select b).SingleOrDefault();

                    if (cartQueDet != null)
                    {
                        try
                        {
                            db.tblCartQueDet.Remove(cartQueDet);
                            db.SaveChanges();
                            return Json(new { status = "OK", reload = false });
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente.", responseConsole = ex.ToString() });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente." });
                    }
                }
            }
            else
            {
                List<CartProductList> productLists = Session["CartList"] as List<CartProductList>;
                var toRemove = productLists.Single(x => x.Code == id);
                productLists.Remove(toRemove);

                if (productLists.Count() > 0)
                {
                    return Json(new { status = "OK", reload = false });
                }
                else
                {
                    Session.Remove("CartList");
                    return Json(new { status = "OK", reload = true });
                }
            }
        }
        [HttpPost]
        public JsonResult DeleteCartList()
        {
            if (Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];

                    tblCartQue cartQue = db.tblCartQue.Where(x => x.refUser == UserId).SingleOrDefault();

                    if(cartQue != null)
                    {
                        try
                        {
                            db.tblCartQue.Remove(cartQue);
                            db.SaveChanges();
                            return Json(new { status = "OK", reload = true });
                        }
                        catch(Exception ex)
                        {
                            return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente.", responseConsole = ex.ToString() });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente." });
                    }
                }
            }
            else
            {
                Session.Remove("CartList");

                if (Session["CartList"] == null)
                {
                    return Json(new { status = "OK", reload = true });
                }
                else
                {
                    return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente." });
                }
            }
        }
        [HttpPost]
        public JsonResult UpdateItemQuantityFromCartList(string code, int quantity)
        {
            if (Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];
                    tblCartQueDet cartQueDet = (from a in db.tblCartQue
                                                join b in db.tblCartQueDet
                                                on a.IdCartQue equals b.refCartQue
                                                where a.refUser == UserId && b.refCodProd == code
                                                select b).SingleOrDefault();

                    if(cartQueDet != null)
                    {
                        try
                        {
                            cartQueDet.Quantity = quantity;
                            db.SaveChanges();
                            return Json(new { status = "OK" });
                        }
                        catch(Exception ex)
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", title = "Ups...!", responseText = "Error al actualizar la cantidad" });
                    }
                }
            }
            else
            {
                try
                {
                    List<CartProductList> productLists = Session["CartList"] as List<CartProductList>;
                    foreach (var i in productLists)
                    {
                        if (i.Code == code)
                        {
                            i.Quantity = quantity;
                            i.SubtotalInt = i.PriceInt * i.Quantity;
                            i.TotalInt = i.PriceOffInt > 0 ? i.PriceOffInt * i.Quantity : i.PriceInt * i.Quantity;
                            i.SubtotalStr = Function.FormatNumber(i.SubtotalInt);
                            i.TotalStr = Function.FormatNumber(i.TotalInt);
                        }
                    }

                    Session["CartList"] = productLists;
                    return Json(new { status = "OK" });
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                }
            }
        }

        public ActionResult CheckOut()
        {
            if(Session["id"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];

                    List<tblCartQueDet> products = (from a in db.tblCartQue
                                                    join b in db.tblCartQueDet
                                                    on a.IdCartQue equals b.refCartQue
                                                    where a.refUser == UserId
                                                    select b).ToList();

                    var viewModel = new Checkout
                    {
                        BindSelect = new BindSelect
                        {
                            countries = db.tblCountry.Select(x => x).ToList(),

                            regions = db.tblRegiones.Select(x => x).ToList(),

                            provinces = db.tblProvincias.Select(x => x).ToList(),

                            comunes = db.tblComunas.Select(x => x).ToList()
                        },
                        ProductList = Function.GetCartProducts(products, db),
                        User = db.tblUsers.Where(x => x.idUser == UserId).SingleOrDefault(),
                        Address = db.tblAddresses.Where(x => x.refuser == UserId && x.boolDefault == true).SingleOrDefault()
                    };

                    return View(viewModel);
                }
                
            }
            else
            {
                TempData["LogIn"] = "Debe iniciar sesión para continuar";
                return RedirectToAction("LogIn", new {language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName });
            }
        }

        public ActionResult LogUp()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn(string ReturnUrl)
        {
            if(Session["email"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ReturnUrl = ReturnUrl;
                return View();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Abandon();
            System.Web.HttpContext.Current.Session.Clear();
            return Json(new { status = "OK" });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LogIn(LogIn logIn)
        {
            if (ModelState.IsValid)
            {
                string logging = LogOn(logIn.Email, logIn.Pass, logIn.RememberMe);
                if (logging == "OK")
                {
                    int UserId = (int)Session["id"];
                    CheckHistory(UserId);
                    CheckQuotingQue(UserId);
                    CheckCartQue(UserId);
                    return Json(new { status = "OK" });
                }
                else if(logging == "invalid")
                {
                    return Json(new { status = "warning", title = "Email no registrado", responseText = "Este usuario no se ha registrado en nuestro sitio web." });
                }else if(logging == "password")
                {
                    return Json(new { status = "warning", title = "Datos de Usuairo", responseText = "Usuario o contraseña no son correctos." });
                }
                else
                {
                    return Json(new { status = "warning", title = "Verificación de Email", responseText = "Para ingresar a su cuenta debe verificar y activar su email, a través de un liink que le fue enviado al su email." });
                }
            }
            else
            {
                return Json(new { status = "error", title = "Ups...!", responseText = "Usuario o contraseña no son correctos." });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult FacebookLogIn(ExternalLogin facebook)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var Email = db.tblUsers.Where(x => x.strEmail == facebook.Email).FirstOrDefault();
                if (Email != null)
                {
                    if(Email.strProvider != "Facebook")
                    {
                        return Json(new { status = "info", title = "Email", responseText = $"El email: <strong>{facebook.Email}</strong> ya ha sido registrado, no como cuenta de Facebook." });
                    }
                    else
                    {
                        string logging = FacebookLogOn(facebook);
                        if (logging == "OK")
                        {
                            CheckHistory(Int32.Parse(Session["id"].ToString()));
                            CheckQuotingQue(Int32.Parse(Session["id"].ToString()));
                            return Json(new { status = "OK" });
                        }
                        else
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = logging });
                        }
                    }
                }
                else
                {
                    string signing = FacebookSignUp(facebook);
                    if(signing == "OK")
                    {
                        string logging = FacebookLogOn(facebook);
                        if (logging == "OK")
                        {
                            CheckHistory(Int32.Parse(Session["id"].ToString()));
                            return Json(new { status = "OK" });
                        }
                        else
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = logging });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", title = "Ups...!", responseText = signing });
                    }
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult GoogleLogIn(ExternalLogin google)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var Email = db.tblUsers.Where(x => x.strEmail == google.Email).FirstOrDefault();
                if (Email != null)
                {
                    if (Email.strProvider != "Google")
                    {
                        return Json(new { status = "info", title = "Email", responseText = $"El email: <strong>{google.Email}</strong> ya ha sido registrado, no como cuenta de Google." });
                    }
                    else
                    {
                        string logging = GoogleLogOn(google);
                        if (logging == "OK")
                        {
                            CheckHistory(Int32.Parse(Session["id"].ToString()));
                            CheckQuotingQue(Int32.Parse(Session["id"].ToString()));
                            return Json(new { status = "OK" });
                        }
                        else
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = logging });
                        }
                    }
                }
                else
                {
                    string signing = GoogleSignUp(google);
                    if (signing == "OK")
                    {
                        string logging = GoogleLogOn(google);
                        if (logging == "OK")
                        {
                            CheckHistory(Int32.Parse(Session["id"].ToString()));
                            return Json(new { status = "OK" });
                        }
                        else
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = logging });
                        }
                    }
                    else
                    {
                        return Json(new { status = "error", title = "Ups...!", responseText = signing });
                    }
                }
            }
        }

        [AllowAnonymous]
        public ActionResult RetrievePassword()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RetrievePasswordEmail(string EmailRecovery)
        {
            webstoreEntities db = new webstoreEntities();
            var user = db.tblUsers.Where(x => x.strEmail == EmailRecovery).FirstOrDefault();
            if ( user != null)
            {
                Random generator = new Random();
                String r = generator.Next(99999, 1000000).ToString("D6");
                String email = SendValidationCode(user.strEmail, r);
                if (email == "Sent")
                {
                    user.strRecoveryCode = r;
                    user.timeRecoveryCode = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        return Json(new { status = "OK", title = "Código", responseText = "Se ha generado un código automáticamente y fue enviado a su dirección de email." }, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex)
                    {
                        return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = email }, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
                return Json(new { status = "warning", title = "Email", responseText = "Email no existe en nuestra base de datos." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetrievePasswordCode(string ValidationCode)
        {
            webstoreEntities db = new webstoreEntities();
            var user = db.tblUsers.Where(x => x.strRecoveryCode == ValidationCode).FirstOrDefault();
            if(user != null)
            {
                if(user.timeRecoveryCode.Value.AddMinutes(30) > DateTime.Now)
                {
                    return Json(new { status = "OK", title = "Validado", responseText = "Código de validación es válido." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "warning", title = "Expiración", responseText = "El código de validación ha expirado." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "error", title = "Código", responseText = "Código de validación incorrecto." }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ChangePassword(RecoveryPassword recovery)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var user = db.tblUsers.Where(x => x.strEmail == recovery.EmailRecovery && x.strRecoveryCode == recovery.CodRecovery).FirstOrDefault();
                if(user != null)
                {
                    if(recovery.PassRecovery == recovery.PassRecoveryConfirmation)
                    {
                        user.strPassword = Crypto.Hash(recovery.PassRecovery);
                        try
                        {
                            db.SaveChanges();
                            return Json(new { status = "OK", title = "Contraseña", responseText = "Su contraseña fue actualizada con éxito." }, JsonRequestBehavior.AllowGet);
                        }
                        catch(Exception ex)
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = "warning", title = "Contraseña", responseText = "Contraseñas no coinciden." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = "Error inesperado al encontrar al usuario." }, JsonRequestBehavior.AllowGet);
                }
            }
            
        }

        public ActionResult Quote()
        {
            if(Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];
                    List<QuotingsProductList> productLists = (from a in db.tblQuotingQue
                                                              join b in db.tblQuotingQueDet
                                                              on a.IdQuotingQue equals b.refQuotingQue
                                                              where a.refUser == UserId
                                                              select new QuotingsProductList
                                                              {
                                                                  Code = b.refCodProd,
                                                                  Quantity = b.Quantity
                                                              }).ToList();
                    if (productLists.Any())
                    {
                        foreach (var i in productLists)
                        {
                            var products = Function.GetProductsList(db).Where(x => x.Codigo == i.Code).Select(x => new Products
                            {
                                productSeo = x.CodigoS,
                                strCodigo = x.Codigo,
                                strNombre = x.Name,
                                intPrecio = Function.FormatNumber(x.Price),
                                intPrecioNum = x.Price,
                                categorySeo = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.Category),
                                Offert = x.Offert,
                                OffertTime = x.OffertTime
                            }).ToList();

                            products = Function.GetOffertOrOffertTime(products, db);

                            var product = products.SingleOrDefault();

                            i.Name = product.strNombre;
                            i.Price = product.intPrecio;
                            i.PriceOff = product.intPrecioOff;
                            i.PercentageOff = product.intPrecentOff;
                            i.Category = product.categorySeo;

                            var stock = Warehouse.GetStock(product.productSeo);

                            foreach(var j in stock)
                            {
                                if(j.TotalStock > 0)
                                {
                                    i.Stock = true;
                                    break;
                                }
                                else
                                {
                                    i.Stock = false;
                                }
                            }
                        }
                    }

                    tblUsers user = db.tblUsers.Where(x => x.idUser == UserId).SingleOrDefault(); 

                    QuotingModelBag Model = new QuotingModelBag
                    {
                        ProductList = productLists,
                        Values = new QuotingForm
                        {
                            QuotingName = user.strNames,
                            QuotingLastname = user.strLastNames,
                            QuotingEmail = user.strEmail,
                            QuotingPhone = (int)user.intPhone
                        }
                    };
                    return View(Model);
                }
            }
            else
            {
                List<QuotingsProductList> productLists = System.Web.HttpContext.Current.Session["QuotingList"] as List<QuotingsProductList>;

                QuotingModelBag Model = new QuotingModelBag
                {
                    ProductList = productLists
                };
                return View(Model);
            }
            //List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;

            //return View(productLists);
        }
        [HttpPost]
        public JsonResult AddProductToQuoting(QuotingsProductList quotingsProduct)
        {
            if(Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = Int32.Parse(Session["id"].ToString());
                    var quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).FirstOrDefault();
                    if(quotingQue != null)
                    {
                        try
                        {
                            string addingToQuotingQueDet = Function.SetQuotingQueDet(db, quotingQue, quotingsProduct);
                            if (addingToQuotingQueDet == "ok")
                            {
                                return Json(new { status = "OK", title = "¡Cotización!", responseText = $"Producto <strong>{quotingsProduct.Code}</strong> fue añadido a la cotización" });
                            }
                            else
                            {
                                return Json(new { status = "error", title = "Ups...!", responseText = addingToQuotingQueDet });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                        }
                    }
                    else
                    {
                        try
                        {
                            tblQuotingQue newQuotingQue = new tblQuotingQue
                            {
                                refUser = UserId
                            };
                            db.tblQuotingQue.Add(newQuotingQue);
                            db.SaveChanges();

                            string addingToQuotingQueDet = Function.SetQuotingQueDet(db, newQuotingQue, quotingsProduct);

                            if(addingToQuotingQueDet == "ok")
                            {
                                return Json(new { status = "OK", title = "¡Cotización!", responseText = $"Producto <strong>{quotingsProduct.Code}</strong> fue añadido a la cotización" });
                            }
                            else
                            {
                                return Json(new { status = "error", title = "Ups...!", responseText = addingToQuotingQueDet});
                            }
                            
                        }catch(Exception ex)
                        {
                            return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                        }
                    }
                }
            }
            else
            {
                try
                {
                    List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;


                    if (productLists != null)
                    {
                        if (productLists.Any(x => x.Code == quotingsProduct.Code))
                        {
                            foreach (var i in productLists)
                            {
                                if (i.Code == quotingsProduct.Code)
                                {
                                    i.Quantity += quotingsProduct.Quantity;
                                }
                            }
                        }
                        else
                        {
                            productLists.Add(quotingsProduct);
                        }
                        Session["QuotingList"] = productLists;
                        return Json(new { status = "OK" });
                    }
                    else
                    {
                        List<QuotingsProductList> Products = new List<QuotingsProductList>
                        {
                        quotingsProduct
                        };
                        Session["QuotingList"] = Products;
                        return Json(new { status = "OK" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", responseText = ex.ToString() });
                }
            }
        }
        [HttpPost]
        public JsonResult RemoveItemFromQuotingList(string id)
        {
            if (Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = Int32.Parse(Session["id"].ToString());
                    tblQuotingQue quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).SingleOrDefault();
                    if(quotingQue != null)
                    {
                        tblQuotingQueDet quotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue && x.refCodProd == id).SingleOrDefault();
                        if(quotingQueDet != null)
                        {
                            db.tblQuotingQueDet.Remove(quotingQueDet);
                            db.SaveChanges();

                            List<tblQuotingQueDet> tblQuotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue).ToList();

                            if (tblQuotingQueDet.Any())
                            {
                                return Json(new { status = "OK", reload = false });
                            }
                            else
                            {
                                db.tblQuotingQue.Remove(quotingQue);
                                db.SaveChanges();
                                return Json(new { status = "OK", reload = true });
                            }
                            
                        }
                        else
                        {
                            return Json(new { status = "warning", title = "Producto", responseText = $"El producto <strong>{id}</strong> no se encuentra en la cotización. Si el problema persiste por favor contáctenos a través de un ticket" });
                        }
                    }
                    else
                    {
                        return Json(new { status = "warning", title = "Producto", responseText = $"El producto <strong>{id}</strong> no se encuentra en la cotización. Si el problema persiste por favor contáctenos a través de un ticket" });
                    }
                }
            }
            else
            {
                List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;
                var toRemove = productLists.Single(x => x.Code == id);
                productLists.Remove(toRemove);

                if (productLists.Count() > 0)
                {
                    return Json(new { status = "OK", reload = false });
                }
                else
                {
                    Session.Remove("QuotingList");
                    return Json(new { status = "OK", reload = true });
                }
            }
        }
        [HttpPost]
        public JsonResult DeleteQuotingList()
        {
            if(Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    try
                    {
                        int UserId = Int32.Parse(Session["id"].ToString());
                        tblQuotingQue quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).SingleOrDefault();
                        if (quotingQue != null)
                        {
                            List<tblQuotingQueDet> quotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue).ToList();
                            foreach (var i in quotingQueDet)
                            {
                                db.tblQuotingQueDet.Remove(i);
                                db.SaveChanges();
                            }
                            db.tblQuotingQue.Remove(quotingQue);
                            db.SaveChanges();
                            return Json(new { status = "OK", reload = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = "error", title = "Up...!", responseText = $"Algo salió mal, no se pudo vaciar su lista, error: <br> {ex.ToString()}" });
                    }
                }
                return Json(new { });
            }
            else
            {
                Session.Remove("QuotingList");

                if (Session["QuotingList"] == null)
                {
                    return Json(new { status = "OK", reload = true });
                }
                else
                {
                    return Json(new { status = "error", title = "Up...!", responseText = "Algo salió mal, no se pudo vaciar su lista, recargue la página e intente nuevamente." });
                }
            }
        }
        [HttpPost]
        public JsonResult UpdateItemQuantityFromQuotingList(string code, int quantity)
        {
            if(Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = Int32.Parse(Session["id"].ToString());
                    tblQuotingQue quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).SingleOrDefault();
                    if(quotingQue != null)
                    {
                        tblQuotingQueDet quotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue && x.refCodProd == code).SingleOrDefault();

                        if(quotingQueDet != null)
                        {
                            try
                            {
                                quotingQueDet.Quantity = quantity;
                                db.SaveChanges();
                                return Json(new { status = "OK" });
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                            }
                        }
                        else
                        {
                            return Json(new { status = "warning", title = "Producto", responseText = $"El producto <strong>{code}</strong> no existe en la cotización" });
                        }
                    }
                    else
                    {
                        return Json(new { status = "warning", title = "cotización", responseText = $"Cotización vacía" });
                    }
                }
            }
            else
            {
                try
                {
                    List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;
                    foreach (var i in productLists)
                    {
                        if (i.Code == code)
                        {
                            i.Quantity = quantity;
                        }
                    }

                    Session["QuotingList"] = productLists;
                    return Json(new { status = "OK" });
                }catch(Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Quoting(QuotingForm Form)
        {
            if(Session["id"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    int UserId = (int)Session["id"];
                    tblQuotingQue quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).SingleOrDefault();
                    if (quotingQue != null)
                    {
                        List<tblQuotingQueDet> quotingQueDet = db.tblQuotingQueDet.Where(x => x.refQuotingQue == quotingQue.IdQuotingQue).ToList();
                        if (quotingQueDet.Any())
                        {
                            try
                            {
                                tblQuotings NewQuoting = new tblQuotings
                                {
                                    strName = Form.QuotingName,
                                    strLastname = Form.QuotingLastname,
                                    strEmail = Form.QuotingEmail,
                                    intPhone = Form.QuotingPhone,
                                    strComment = Form.QuotingComment,
                                    strDate = DateTime.Now,
                                    refUser = UserId
                                };

                                db.tblQuotings.Add(NewQuoting);
                                db.SaveChanges();
                                try
                                {
                                    foreach (var i in quotingQueDet)
                                    {
                                        tblQuotingsDet NewQuotingDet = new tblQuotingsDet
                                        {
                                            refQuoting = NewQuoting.IdQuoting,
                                            refProduct = i.refCodProd,
                                            intQuantity = i.Quantity
                                        };
                                        db.tblQuotingsDet.Add(NewQuotingDet);
                                        db.SaveChanges();
                                    }

                                    string sendEmail = SendQuotingEmail(Form.QuotingEmail, quotingQueDet, NewQuoting.IdQuoting, NewQuoting.strDate, db);
                                    if(sendEmail == "sent")
                                    {
                                        foreach (var i in quotingQueDet)
                                        {
                                            db.tblQuotingQueDet.Remove(i);
                                        }
                                        db.tblQuotingQue.Remove(quotingQue);
                                        db.SaveChanges();
                                        return Json(new { status = "OK", title = "Cotización", responseText = "Su cotización fue realizada con éxito, uno de nuestros agentes se contactará con usted en lo más breve posible.<br> Un email de confirmación le fue enviado a su dirección de correo.<br> Gracias por contar con nosotros." });
                                    }
                                    else
                                    {
                                        return Json(new { status = "waning", title = "Email", responseText = $"Su cotización fue realizada con éxito, uno de nuestros agentes se contactará con usted en lo más breve posible.<br> Pero ocurrió una falla al intentar enviar un email de confirmación a su dirección de correo.<br>Pero no se preocupe su cotización fue enviada a nuestro sistema.", consoleText = sendEmail });
                                    }
                                    
                                }
                                catch (Exception ex)
                                {
                                    return Json(new { status = "error", title = "Ups...!", responseText = $"Se generó un error al registrar los productos de su cotización, por favor de ser posible contacte con nuestro soporte a través de un ticket.", consoleText = ex.ToString() });
                                }
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = "error", title = "Ups...!", responseText = $"Se generó un error al registrar su cotización, por favor de ser posible contacte con nuestro soporte a través de un ticket.", consoleText = ex.ToString() });
                            }
                        }
                        else
                        {
                            return Json(new { status = "warning", title = "Productos", responseText = "No tiene productos para añadir a su cotización, por favor intente nuevamente.<br> Si el problema persiste por favor contacte con nosotros a través de un ticket" });
                        }
                    }
                    else { return Json(new { status = "warning", title = "Cotización", responseText = "No tiene ninguna cotización para enviar, por favor intente nuevamente.<br> Si el problema persiste por favor contacte con nosotros a través de un ticket" }); }
                }
            }
            else
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    List<QuotingsProductList> productLists = System.Web.HttpContext.Current.Session["QuotingList"] as List<QuotingsProductList>;

                    if (productLists.Any())
                    {
                        try
                        {
                            tblQuotings NewQuoting = new tblQuotings
                            {
                                strName = Form.QuotingName,
                                strLastname = Form.QuotingLastname,
                                strEmail = Form.QuotingEmail,
                                intPhone = Form.QuotingPhone,
                                strComment = Form.QuotingComment,
                                strDate = DateTime.Now
                            };

                            db.tblQuotings.Add(NewQuoting);
                            db.SaveChanges();
                            try
                            {
                                List<tblQuotingQueDet> queDet = new List<tblQuotingQueDet>();

                                foreach(var i in productLists)
                                {
                                    queDet.Add(new tblQuotingQueDet
                                    {
                                        Quantity = i.Quantity,
                                        refCodProd = i.Code
                                    });
                                }
                                
                                string sendEmail = SendQuotingEmail(Form.QuotingEmail, queDet, NewQuoting.IdQuoting, NewQuoting.strDate, db);
                                if (sendEmail == "Sent")
                                {
                                    foreach (var i in productLists)
                                    {
                                        tblQuotingsDet NewQuotingDet = new tblQuotingsDet
                                        {
                                            refQuoting = NewQuoting.IdQuoting,
                                            refProduct = i.Code,
                                            intQuantity = i.Quantity
                                        };
                                        db.tblQuotingsDet.Add(NewQuotingDet);
                                        db.SaveChanges();
                                    }
                                    Session.Remove("QuotingList");
                                    return Json(new { status = "OK", title = "Cotización", responseText = "Su cotización fue realizada con éxito, uno de nuestros agentes se contactará con usted en lo más breve posible.<br> Un email de confirmación le fue enviado a su dirección de correo.<br> Gracias por contar con nosotros." });
                                }
                                else
                                {
                                    return Json(new { status = "warning", title = "Email", responseText = $"Su cotización fue realizada con éxito, uno de nuestros agentes se contactará con usted en lo más breve posible.<br> Pero ocurrió una falla al intentar enviar un email de confirmación a su dirección de correo.<br>Pero no se preocupe su cotización fue enviada a nuestro sistema.", consoleText = sendEmail });
                                }
                            }
                            catch (Exception ex)
                            {
                                return Json(new { status = "error", title = "Ups...!", responsetext = $"Se generó un error al registrar los productos de su cotización, por favor de ser posible contacte con nuestro soporte a través de un ticket.", consoleText = ex.ToString() });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { status = "error", title = "Ups...!", responsetext = $"Se generó un error al registrar su cotización, por favor de ser posible contacte con nuestro soporte a través de un ticket.", consoleText = ex.ToString() });
                        }
                    }
                    else
                    {
                        return Json(new { status = "warning", title = "Cotización", responseText = "No tiene ninguna cotización para enviar, por favor intente nuevamente.<br> Si el problema persiste por favor contacte con nosotros a través de un ticket" });
                    }
                }
            }
        }

        public ActionResult Continue(string id)
        {
            return RedirectToAction("Details", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(Registration user)
        {

            try
            {
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist.");
                    return Json(new
                    {
                        status = "warning",
                        title = "Email",
                        responseText = $"El email <strong>{user.Email}</strong> ya está siendo usado."
                    });
                }

                if(user.Pass != user.Passre)
                {
                    ModelState.AddModelError("Password", "Contraseñas no coinciden.");
                    return Json(new
                    {
                        status = "warning",
                        title = "Password",
                        responseText = "Las contraseñas no coinciden."
                    });
                }

                if (IsRutExist(user.Id))
                {
                    ModelState.AddModelError("Id", "RUT.");
                    return Json(new
                    {
                        status = "warning",
                        title = "RUT",
                        responseText = "Su número de rut está siendo usado por otra persona, si no es uested por favor contáctenos a través d eun ticket de soporte."
                    });
                }

                using (webstoreEntities db = new webstoreEntities())
                {
                    tblUsers nickObj = new tblUsers
                    {
                        intId = user.Id,
                        strNames = user.Name.ToLower(),
                        strLastNames = user.Lastname.ToLower(),
                        strEmail = user.Email.ToLower(),
                        intPhone = user.Phone,
                        strVerificationCode = Guid.NewGuid(),
                        strPassword = Crypto.Hash(user.Pass),
                        intLevel = 1,
                        boolValidate = false,
                        strRegistrationDate = DateTime.Now,
                        strProvider = "Local"
                    };
                    db.tblUsers.Add(nickObj);
                    tblAddresses newAddress = new tblAddresses
                    {
                        strCountry = "Chile",
                        strCity = user.City,
                        refRegion = user.States,
                        refProvince = user.Provinces,
                        refComuna = user.Comunes,
                        strAddress = user.AddressOne,
                        strType = user.Type,
                        refuser = nickObj.idUser,
                        boolDefault = true,
                        boolThird = false
                    };

                    db.tblAddresses.Add(newAddress);

                    if(user.RegistrationType == 2)
                    {
                        var company = db.tblCompany.Where(x => x.intId == user.Company.CompanyId).SingleOrDefault();

                        if(company == null)
                        {
                            tblCompany newCompany = new tblCompany
                            {
                                intId = user.Company.CompanyId,
                                strName = user.Company.CompanyName,
                                strActivity = user.Company.CompanyActivity,
                                intPhone = user.Company.CompanyPhone,
                                refRegion = user.Company.CompanyStates,
                                refProvince = user.Company.CompanyProvinces,
                                refComune = user.Company.CompanyComunes,
                                strCity = user.Company.CompanyCity,
                                strAddress = user.Company.CompanyAddressOne
                            };

                            db.tblCompany.Add(newCompany);

                            tblRelCompanyUser newRelation = new tblRelCompanyUser
                            {
                                refUser = nickObj.idUser,
                                refCompany = newCompany.idCompany
                            };

                            db.tblRelCompanyUser.Add(newRelation);
                        }
                        else
                        {
                            tblRelCompanyUser newRelation = new tblRelCompanyUser
                            {
                                refUser = nickObj.idUser,
                                refCompany = company.idCompany
                            };

                            db.tblRelCompanyUser.Add(newRelation);
                        }
                    }

                    var j = SendVerificationLinkEmail(user.Email, nickObj.strVerificationCode.ToString());
                    if (j == "Sent")
                    {
                        try
                        {
                            db.SaveChanges();
                            return Json(new
                            {
                                status = "OK",
                                title = "Registro Exitoso..!",
                                responseText = "Registro realizado con éxito. El link para activar su cuenta ha sido enviado a la siguiente dirección de email:" + user.Email
                            });
                        }
                        catch(Exception ex)
                        {
                            return Json(new
                            {
                                status = "error",
                                title = "Upss..!",
                                responseText = ex.ToString()
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            status = "warning",
                            title = "Codigo de Verificación",
                            responseText = "Registro realizado con éxito. Pero ocurrió un problema al enviarle un link de confirmación, para solicitar uno nuevo por favor ingrese a su cuenta y haga clic en solicitar link de verificación " + j
                        });
                    }
                }
            }
            catch (Exception x)
            {
                return Json(new
                {
                    status = "error",
                    title = "Upss..!",
                    responseText = x.ToString()
                });
            }
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (webstoreEntities db = new webstoreEntities())
            {
                db.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
                                                                // Confirm password does not match issue on save changes
                var v = db.tblUsers.Where(a => a.strVerificationCode == new Guid(id)).FirstOrDefault();
                if (v != null)
                {
                    try
                    {
                        v.boolValidate = true;
                        db.SaveChanges();
                        Status = true;
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Message = ex.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "Código de activación no válido.";
                }
            }
            ViewBag.Status = Status;
            ViewBag.userCode = id;
            return View();
        }

        public ActionResult History()
        {
            ViewBag.Mobile = Request.Browser.IsMobileDevice;

            if (Session["id"] != null)
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                using(webstoreEntities db = new webstoreEntities())
                {
                    tblHistory History = db.tblHistory.Where(x => x.refUser == UserId).FirstOrDefault();
                    if(History != null)
                    {
                        List<Products> x = new List<Products>();

                        var HistoryDet = (from a in db.tblHistoryDet
                                          join b in db.tblProducts
                                          on a.refProduct equals b.idProduct
                                          where a.refHistory == History.IdHistory
                                          select b).ToList();
                        foreach(var i in HistoryDet)
                        {
                            var product = Function.GetHistoryMainItems(i.strCode, db);

                            if (x.Any())
                            {
                                x.Insert(0, product);
                            }
                            else
                            {
                                x.Add(product);
                            }
                        }

                        x = Function.GetOffertOrOffertTime(x, db);

                        return View(x);
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            else
            {
                if(Request.Cookies["History"] != null)
                {
                    using (webstoreEntities db = new webstoreEntities())
                    {
                        var History = Request.Cookies["History"];
                        var items = History.Values.AllKeys.SelectMany(History.Values.GetValues, (k, v) => new { key = k, value = v });
                        List<Products> x = new List<Products>();
                        foreach (var w in items)
                        {
                            var product = Function.GetHistoryMainItems(w.value, db);

                            if (x.Any())
                            {
                                x.Insert(0, product);
                            }
                            else
                            {
                                x.Add(product);
                            }

                        }

                        x = Function.GetOffertOrOffertTime(x, db);

                        return View(x);
                    }
                }
                return View();
            }
            
        }

        [NonAction]
        private string SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            var verifyUrl = "http://eeppwebstore.ddns.net/"+culture+"/"+(culture == "en"? "User/VerifyAccount":"Usuario/VerificarCuenta")+"/"+activationCode;

            var toEmail = new MailAddress(emailID);
            string subject = "Su cuenta ha sido creada exitosamente";

            string body = "<br/><br/>Nos complace anunciarte que tu cuanta de EEPP" +
                " ha sido creada con éxito. Por favor has clic en el link de abajo para verificar tu cuenta." +
                " <br/><br/><a href='" + verifyUrl + "' class='flat'>Activar Email </a> ";

            var smtp = Configuration.GetSmtp();
            var message = Function.GenerateEmail(toEmail, subject, body);
            try
            {
                smtp.Send(message);
                return "Sent";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string SendValidationCode(string Email, string r)
        {
            using (var smtp = Configuration.GetSmtp()) {
                var toEmail = new MailAddress(Email);
                string subject = "Solicitud para restablecer contraseña";
                string body = $@"<br/><br/>Hemos recibido una solicitud para restablecer su contraseña.
                            <br/><br/>Se ha generado un código de verificación automático con un tiempo de expiración de 30 minutos.
                            <br/>Debe ingresar este código en la casilla de nuestra página web y proseguir con los pasos.
                            <br/>Su codigo de validación es:
                            <br/><br/><strong>{r}</strong>";
                try
                {
                    MailMessage message = Function.GenerateEmail(toEmail, subject, body);
                    smtp.Send(message);
                    return "Sent";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
            
        }

        private string SendQuotingEmail(string Email, List<tblQuotingQueDet> products, int num, DateTime date, webstoreEntities db)
        {
            using (var smtp = Configuration.GetSmtp())
            {
                var toEmail = new MailAddress(Email);
                string subject = "Confirmación de cotización";
                string body = $@"<br/><br/>Hemos recibido una solicitud de cotización con los siguientes datos:
                            <br/><br/>Número de cotización: {num}.
                            <br/>Fecha de cotización {date.ToString("dd/MM/yyyy HH:mm:ss")}.
                            <br/><br/><h4>Detalles de productos</h4>:
                            <br/><br/>
                            <table class='shopping-cart table'>
                                <thead>
                                    <tr>
                                        <th>Producto</th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>";

                foreach (var i in products)
                {
                    var productsInfo = Function.GetProductsList(db).Where(x => x.Codigo == i.refCodProd).Select(x => new QuotingsProductList
                    {
                        Code = x.Codigo,
                        Name = x.Name,
                        Quantity = i.Quantity
                    }).SingleOrDefault();

                    body += $@"
                            <tr>
                                <td>
                                    <div>
                                        <a>
                                            <img src='http://eeppwebstore.ddns.net/Content/img/products/{productsInfo.Code}-1.jpg' alt='Product'>
                                        </a>
                                        <div>
                                            <h4>
                                                <a>{productsInfo.Name}</a>
                                            </h4>
                                        </div>
                                    </div>
                                </td>
                                <td>{productsInfo.Quantity}</td>
                            </tr>";
                }

                body += "</tbody></table>";
                try
                {
                    MailMessage message = Function.GenerateEmail(toEmail, subject, body, "1000");
                    smtp.Send(message);
                    return "Sent";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        [NonAction]
        private bool IsEmailExist(string emailID)
        {
            using (webstoreEntities dc = new webstoreEntities())
            {
                var v = dc.tblUsers.Where(a => a.strEmail == emailID).FirstOrDefault();
                return v != null;
            }
        }
        private bool IsRutExist(string ID)
        {
            using (webstoreEntities dc = new webstoreEntities())
            {
                var v = dc.tblUsers.Where(a => a.intId == ID).SingleOrDefault();
                return v != null ? true : false;
            }
        }

        private string LogOn(string userName, string password, bool rememberMe)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                var valid = db.tblUsers.Where(x => x.strEmail == userName).FirstOrDefault();
                if (valid != null)
                {
                    if (String.Compare(Crypto.Hash(password), valid.strPassword) == 0)
                    {
                        if (valid.boolValidate == true)
                        {
                            var avatar = db.tblAvatars.Where(x => x.refUser == valid.idUser).FirstOrDefault();
                            CookieHelper newCookieHelper = new CookieHelper(HttpContext.Request, HttpContext.Response);
                            newCookieHelper.SetLoginCookie(userName, password, rememberMe);
                            System.Web.HttpContext.Current.Session["lvl"] = valid.intLevel;
                            System.Web.HttpContext.Current.Session["id"] = valid.idUser;
                            System.Web.HttpContext.Current.Session["email"] = valid.strEmail;
                            System.Web.HttpContext.Current.Session["names"] = valid.strNames;
                            System.Web.HttpContext.Current.Session["lastname"] = valid.strLastNames;
                            System.Web.HttpContext.Current.Session["date"] = valid.strRegistrationDate;
                            System.Web.HttpContext.Current.Session["Provider"] = "Local";
                            if (avatar != null)
                            {
                                System.Web.HttpContext.Current.Session["img"] = avatar.strAvatarName;
                                System.Web.HttpContext.Current.Session["imgType"] = avatar.strAvatarType;
                            }

                            return "OK";
                        }
                        else
                        {
                            return "email";
                        }
                    }
                    else
                    {
                        return "password";
                    }
                }
                else
                {
                    return "invalid";
                }
            }
            
        }

        private string FacebookLogOn(ExternalLogin facebook)
        {
            try
            {
                CookieHelper newCookieHelper = new CookieHelper(HttpContext.Request, HttpContext.Response);
                newCookieHelper.SetLoginCookie(facebook.Email, "", false);
                webstoreEntities db = new webstoreEntities();
                var user = db.tblUsers.Where(x => x.strEmail == facebook.Email).FirstOrDefault();
                System.Web.HttpContext.Current.Session["id"] = user.idUser;
                System.Web.HttpContext.Current.Session["idFacebook"] = facebook.Id;
                System.Web.HttpContext.Current.Session["email"] = facebook.Email;
                System.Web.HttpContext.Current.Session["names"] = user.strNames;
                System.Web.HttpContext.Current.Session["lastname"] = user.strLastNames;
                System.Web.HttpContext.Current.Session["Provider"] = "Facebook";
                System.Web.HttpContext.Current.Session["date"] = user.strRegistrationDate;

                return "OK";
            }catch(Exception ex)
            {
                return ex.ToString();
            }
        }

        private string GoogleLogOn(ExternalLogin google)
        {
            try
            {
                CookieHelper newCookieHelper = new CookieHelper(HttpContext.Request, HttpContext.Response);
                newCookieHelper.SetLoginCookie(google.Email, "", false);
                webstoreEntities db = new webstoreEntities();
                var user = db.tblUsers.Where(x => x.strEmail == google.Email).FirstOrDefault();
                System.Web.HttpContext.Current.Session["id"] = user.idUser;
                System.Web.HttpContext.Current.Session["idGoogle"] = google.Id;
                System.Web.HttpContext.Current.Session["email"] = google.Email;
                System.Web.HttpContext.Current.Session["names"] = user.strNames;
                System.Web.HttpContext.Current.Session["lastname"] = user.strLastNames;
                System.Web.HttpContext.Current.Session["Provider"] = "Google";
                System.Web.HttpContext.Current.Session["date"] = user.strRegistrationDate;
                System.Web.HttpContext.Current.Session["img"] = google.Photo;
                System.Web.HttpContext.Current.Session["imgType"] = "img";
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private string FacebookSignUp(ExternalLogin facebook)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                tblUsers newUser = new tblUsers
                {
                    strEmail = facebook.Email,
                    strNames = facebook.Name.Split(' ')[0],
                    strLastNames = facebook.Name.Split(' ')[1],
                    strProvider = "Facebook",
                    strRegistrationDate = DateTime.Now,
                    boolValidate = true,
                    intLevel = 1
                };
                try
                {
                    db.tblUsers.Add(newUser);
                    db.SaveChanges();
                    return "OK";
                }catch(Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        private string GoogleSignUp(ExternalLogin google)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                tblUsers newUser = new tblUsers
                {
                    strEmail = google.Email,
                    strNames = google.Name.Split(' ')[0],
                    strLastNames = google.Name.Split(' ')[1],
                    strProvider = "Google",
                    strRegistrationDate = DateTime.Now,
                    boolValidate = true,
                    intLevel = 1
                };
                try
                {
                    db.tblUsers.Add(newUser);
                    db.SaveChanges();
                    return "OK";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }

        private void CheckHistory(int UserId)
        {
            if(Request.Cookies["History"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    var HistoryCookie = Request.Cookies["History"];
                    var items = HistoryCookie.Values.AllKeys.SelectMany(HistoryCookie.Values.GetValues, (k, v) => new { key = k, value = v });
                    var History = db.tblHistory.Where(x => x.refUser == UserId).FirstOrDefault();
                    if (History != null)
                    {
                        foreach(var i in items)
                        {
                            var HistoryDet = (from a in db.tblHistoryDet
                                              join b in db.tblProducts
                                              on a.refProduct equals b.idProduct
                                              where a.refHistory == History.IdHistory && b.strCode == i.value
                                              select new
                                              {
                                                  Codigo = a.refProduct
                                              }).FirstOrDefault();
                            if(HistoryDet == null)
                            {
                                tblHistoryDet newHistoryDet = new tblHistoryDet()
                                {
                                    refHistory = History.IdHistory,
                                    refProduct = db.tblProducts.Where(x => x.strCode == i.value).FirstOrDefault().idProduct
                                };
                                db.tblHistoryDet.Add(newHistoryDet);
                                db.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        tblHistory newHistory = new tblHistory
                        {
                            refUser = UserId
                        };
                        db.tblHistory.Add(newHistory);
                        db.SaveChanges();
                        foreach (var i in items)
                        {
                            var HistoryDet = (from a in db.tblHistoryDet
                                              join b in db.tblProducts
                                              on a.refProduct equals b.idProduct
                                              where a.refHistory == newHistory.IdHistory && b.strCode == i.value
                                              select new
                                              {
                                                  Codigo = a.refProduct
                                              }).FirstOrDefault();
                            if (HistoryDet == null)
                            {
                                tblHistoryDet newHistoryDet = new tblHistoryDet()
                                {
                                    refHistory = newHistory.IdHistory,
                                    refProduct = db.tblProducts.Where(x => x.strCode == i.value).FirstOrDefault().idProduct
                                };
                                db.tblHistoryDet.Add(newHistoryDet);
                                db.SaveChanges();
                            }
                        }
                    }

                    HistoryCookie.Expires = DateTime.Now.AddDays(-1);
                }
            }
        }
        private void CheckQuotingQue(int UserId)
        {
            if(Session["QuotingList"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;
                    tblQuotingQue quotingQue = db.tblQuotingQue.Where(x => x.refUser == UserId).SingleOrDefault();

                    if(quotingQue != null)
                    {
                        foreach (var i in productLists)
                        {
                            string addingToQuotingQueDet = Function.SetQuotingQueDet(db, quotingQue, i);
                        }
                    }
                    else
                    {
                        tblQuotingQue newQuotingQue = new tblQuotingQue
                        {
                            refUser = UserId
                        };
                        db.tblQuotingQue.Add(newQuotingQue);
                        db.SaveChanges();

                        foreach(var i in productLists)
                        {
                            string addingToQuotingQueDet = Function.SetQuotingQueDet(db, newQuotingQue, i);
                        }
                    }

                    Session.Remove("QuotingList");
                }
            }
        }
        private void CheckCartQue(int UserId)
        {
            if(Session["CartList"] != null)
            {
                using (webstoreEntities db = new webstoreEntities())
                {
                    List<CartProductList> productLists = Session["CartList"] as List<CartProductList>;
                    tblCartQue cartQue = db.tblCartQue.Where(x => x.refUser == UserId).SingleOrDefault();
                    if (cartQue != null)
                    {
                        foreach(var i in productLists)
                        {
                            string addToCart = Function.SetCartQueDet(db, cartQue, i);
                        }
                    }
                    else
                    {
                        tblCartQue newCartQue = new tblCartQue
                        {
                            refUser = UserId
                        };
                        db.tblCartQue.Add(newCartQue);
                        db.SaveChanges();

                        foreach(var i in productLists)
                        {
                            string addToCart = Function.SetCartQueDet(db, newCartQue, i);
                        }
                    }

                    Session.Remove("CartList");
                }
            }
        }

    }
}