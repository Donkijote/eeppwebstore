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

namespace WebStore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Cart()
        {
            return View();
        }

        public ActionResult CheckOut()
        {
            webstoreEntities db = new webstoreEntities();
            var viewModel = new BindSelect();

            viewModel.countries = db.tblCountry.Select(x => x).ToList();

            viewModel.regions = db.tblRegiones.Select(x => x).ToList();

            viewModel.comunes = db.tblComunas.Where(x => x.refProvincia == 1).ToList();

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult LogIn()
        {
            if(Session["email"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
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
            /*if(Session["id"] != null)
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int UserId = Int32.Parse(Session["id"].ToString());
                    List<QuotingsProductList> productLists = (from a in db.tblQuotingQue
                                                              join b in db.tblQuotingQueDet
                                                              on a.IdQuotingQue equals b.refQuotingQue
                                                              where a.refUser == UserId
                                                              select new QuotingsProductList
                                                              {
                                                                  Code = b.refCodProd,
                                                                  Quantity = b.Quantity
                                                              }).ToList();
                    return View(productLists);
                }
            }
            else
            {
                List<QuotingsProductList> productLists = System.Web.HttpContext.Current.Session["QuotingList"] as List<QuotingsProductList>;

                return View(productLists);
            }*/
            List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;

            return View(productLists);
        }
        [HttpPost]
        public JsonResult AddProductToQuoting(QuotingsProductList quotingsProduct)
        {
            try
            {
                List<QuotingsProductList> productLists = Session["QuotingList"] as List<QuotingsProductList>;
                

                if(productLists != null)
                {
                    if(productLists.Any(x => x.Code == quotingsProduct.Code))
                    {
                        foreach(var i in productLists)
                        {
                            if(i.Code == quotingsProduct.Code)
                            {
                                i.Quantity += quotingsProduct.Quantity;
                            }
                        }
                    }
                    else{
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
            }catch(Exception ex)
            {
                return Json(new { status = "error", responseText = ex.ToString() });
            }
        }
        [HttpPost]
        public JsonResult RemoveItemFromQuotingList(string id)
        {
            if (Session["id"] != null)
            {
                return Json(new { });
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
                return Json(new { });
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
                    }, JsonRequestBehavior.AllowGet);
                }

                if(user.Pass != user.Passre)
                {
                    ModelState.AddModelError("Password", "Contraseñas no coinciden.");
                    return Json(new
                    {
                        status = "warning",
                        title = "Password",
                        responseText = "Las contraseñas no coinciden."
                    }, JsonRequestBehavior.AllowGet);
                }

                tblUsers nickObj = new tblUsers
                {
                    strNames = user.Name.ToLower(),
                    strLastNames = user.Lastname.ToLower(),
                    strEmail = user.Email.ToLower(),
                    strVerificationCode = Guid.NewGuid(),
                    strPassword = Crypto.Hash(user.Pass),
                    intLevel = 1,
                    boolValidate = false,
                    strRegistrationDate = DateTime.Now,
                    strProvider = "Local"
                };

                using (webstoreEntities db = new webstoreEntities())
                {
                    var j = SendVerificationLinkEmail(user.Email, nickObj.strVerificationCode.ToString());
                    if (j == "Sent")
                    {
                        try
                        {
                            db.tblUsers.Add(nickObj);
                            db.SaveChanges();
                            return Json(new
                            {
                                status = "OK",
                                title = "Registro Exitoso..!",
                                responseText = "Registro realizado con éxito. El link para activar su cuenta ha sido enviado a la siguiente dirección de email:" + user.Email
                            }, JsonRequestBehavior.AllowGet);
                        }
                        catch(Exception ex)
                        {
                            return Json(new
                            {
                                status = "error",
                                title = "Upss..!",
                                responseText = ex.ToString()
                            }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            status = "error",
                            title = "Codigo de Verificación",
                            responseText = j
                        }, JsonRequestBehavior.AllowGet);
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
                }, JsonRequestBehavior.AllowGet);
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

        [NonAction]
        private String SendVerificationLinkEmail(string emailID, string activationCode)
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

        private String SendValidationCode(string Email, string r)
        {
            var smtp = Configuration.GetSmtp();
            var toEmail = new MailAddress(Email);
            string subject = "Solicitud para restablecer contraseña";
            string body = $@"<br/><br/>Hemos recibido una solicitud para restablecer su contraseña.
                            <br/><br/>Se ha generado un código de verificación automático con un tiempo de expiración de 30 minutos.
                            <br/>Debe ingresar este código en la casilla de nuestra página web y proseguir con los pasos.
                            <br/>Su codigo de validación es:
                            <br/><br/><strong>{r}</strong>";
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

        [NonAction]
        private bool IsEmailExist(string emailID)
        {
            using (webstoreEntities dc = new webstoreEntities())
            {
                var v = dc.tblUsers.Where(a => a.strEmail == emailID).FirstOrDefault();
                return v != null;
            }
        }

        private string LogOn(string userName, string password, bool rememberMe)
        {
            webstoreEntities db = new webstoreEntities();
            var valid = db.tblUsers.Where(x => x.strEmail == userName).FirstOrDefault();
            if (valid != null)
            {
                if(String.Compare(Crypto.Hash(password), valid.strPassword) == 0){
                    if (valid.boolValidate == true)
                    {
                        CookieHelper newCookieHelper = new CookieHelper(HttpContext.Request, HttpContext.Response);
                        newCookieHelper.SetLoginCookie(userName, password, rememberMe);
                        System.Web.HttpContext.Current.Session["lvl"] = valid.intLevel;
                        System.Web.HttpContext.Current.Session["id"] = valid.idUser;
                        System.Web.HttpContext.Current.Session["email"] = valid.strEmail;
                        System.Web.HttpContext.Current.Session["names"] = valid.strNames;
                        System.Web.HttpContext.Current.Session["lastname"] = valid.strLastNames;
                        System.Web.HttpContext.Current.Session["Provider"] = "Local";
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

        private string FacebookLogOn(ExternalLogin facebook)
        {
            try
            {
                CookieHelper newCookieHelper = new CookieHelper(HttpContext.Request, HttpContext.Response);
                newCookieHelper.SetLoginCookie(facebook.Email, "", false);
                System.Web.HttpContext.Current.Session["id"] = facebook.Id;
                System.Web.HttpContext.Current.Session["email"] = facebook.Email;
                System.Web.HttpContext.Current.Session["names"] = facebook.Name.Split(' ')[0];
                System.Web.HttpContext.Current.Session["lastname"] = facebook.Name.Split(' ')[1];
                System.Web.HttpContext.Current.Session["Provider"] = "Facebook";
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
                System.Web.HttpContext.Current.Session["id"] = google.Id;
                System.Web.HttpContext.Current.Session["email"] = google.Email;
                System.Web.HttpContext.Current.Session["names"] = google.Name.Split(' ')[0];
                System.Web.HttpContext.Current.Session["lastname"] = google.Name.Split(' ')[1];
                System.Web.HttpContext.Current.Session["Provider"] = "Google";
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

    }
}