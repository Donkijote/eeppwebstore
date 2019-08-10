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
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.Abandon();
            System.Web.HttpContext.Current.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LogIn(LogIn logIn)
        {
            if (ModelState.IsValid)
            {
                string logging = LogOn(logIn.Email.ToLower(), logIn.Pass.ToLower(), logIn.RememberMe);
                if (logging == "OK")
                {
                    return Json(new { status = "OK" });
                }
                else if(logging == "invalid")
                {
                    return Json(new { status = "invalid", title = "Email no registrado", responseText = "Este usuario no se ha registrado en nuestro sitio web." });
                }else if(logging == "password")
                {
                    return Json(new { status = "password", title = "Datos de Usuairo", responseText = "Usuario o contraseña no son correctos." });
                }
                else
                {
                    return Json(new { status = "email", title = "Verificación de Email", responseText = "Para ingresar a su cuenta debe verificar y activar su email, a través de un liink que le fue enviado al su email." });
                }
            }
            else
            {
                return Json(new { status = "error", title = "Ups...!", responseText = "Usuario o contraseña no son correctos." });
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
                if (email == "sent")
                {
                    user.strRecoveryCode = r;
                    user.timeRecoveryCode = DateTime.Now;
                    try
                    {
                        db.SaveChanges();
                        return Json(new { status = "OK", responseText = "something" }, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex)
                    {
                        return Json(new { status = "error", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "error", responseText = email }, JsonRequestBehavior.AllowGet);
                }
                
            }
            else
            {
                return Json(new { status = "error", responseText = "Email no existe en nuestra base de datos." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetrievePasswordCode(string ValidationCode)
        {
            webstoreEntities db = new webstoreEntities();
            var user = db.tblUsers.Where(x => x.strRecoveryCode == ValidationCode).FirstOrDefault();
            if(user != null)
            {
                if(user.timeRecoveryCode > DateTime.Now)
                {
                    return Json(new { status = "OK", responseText = "Código de validación es válido." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "warning", responseText = "El código de validación ha expirado." }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = "error", responseText = "Código de validación incorrecto." }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Quote()
        {
            return View();
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
                        status = "email",
                        title = "Ups...!",
                        responseText = "El email ya está siendo usado."
                    }, JsonRequestBehavior.AllowGet);
                }

                if(user.Pass != user.Passre)
                {
                    ModelState.AddModelError("Password", "Contraseñas no coinciden.");
                    return Json(new
                    {
                        status = "password",
                        title = "Ups...!",
                        responseText = "Las contraseñas no coinciden."
                    }, JsonRequestBehavior.AllowGet);
                }

                tblUsers nickObj = new tblUsers();

                nickObj.strNames = user.Name.ToLower();
                nickObj.strLastNames = user.Lastname.ToLower();
                nickObj.strEmail = user.Email.ToLower();
                nickObj.strVerificationCode = Guid.NewGuid();
                nickObj.strPassword = Crypto.Hash(user.Pass);
                nickObj.intLevel = 1;
                nickObj.boolValidate = false;

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
}