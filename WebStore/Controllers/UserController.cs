using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using WebStore.Functions;

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

        public ActionResult LogIn()
        {
            return View();
        }

        public ActionResult Quote()
        {
            return View();
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

                if(user.Pass.ToLower() != user.Passre.ToLower())
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
                nickObj.strPassword = user.Pass.ToLower();
                nickObj.strVerificationCode = Guid.NewGuid();
                nickObj.strPassword = Crypto.Hash(nickObj.strPassword.ToLower());
                nickObj.boolValidate = false;

                using (webstoreEntities db = new webstoreEntities())
                {
                    db.tblUsers.Add(nickObj);
                    db.SaveChanges();
                    SendVerificationLinkEmail(user.Email, nickObj.strVerificationCode.ToString());
                    return Json(new
                    {
                        status = "OK",
                        title = "Registro Exitoso..!",
                        responseText = "Registro realizado con éxito. El link para activar su cuenta ha sido enviado a la siguiente dirección de email:" + user.Email
                    }, JsonRequestBehavior.AllowGet);
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
                    v.boolValidate = true;
                    db.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [NonAction]
        private void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("eepp@eepp.cl", "Electro Productos");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "eepp.2019."; // Replace with actual password
            string subject = "Su cuenta ha sido creada exitosamente";

            string body = "<br/><br/>Nos complace anunciarte que tu cuanta de EEPP" +
                " ha sido creada con éxito. Por favor has clic en el link de abajo para verificar tu cuenta." +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "mail.eepp.cl",
                Port = 21,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
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
        
    }
}