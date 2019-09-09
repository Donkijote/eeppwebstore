using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore.Models;
using System.Threading;
using System.Globalization;
using System.Web.Routing;
using WebStore.Functions;
using System.Net.Mail;

namespace WebStore.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Home()
        {

            string url = Request.RawUrl;
            string[] location = url.Split('/');

            ViewBag.url = location[2];

            return View();
        }

        public ActionResult Orders(string id = null)
        {

            ViewBag.url = "Orders";

            if (String.IsNullOrWhiteSpace(id))
            {
                ViewBag.orderType = "Order";
                return View();
            }
            else if(id == "op")
            {
                ViewBag.orderType = "OpenOrders";
                return View();
            }
            else

            {
                ViewBag.orderType = "NullOrders";
                return View();
            }      
        }

        public ActionResult Traking()
        {

            ViewBag.url = "Traking";

            return View();
        }

        public ActionResult Details(string x)
        {
            return View();
        }

        public ActionResult Address(string id = null)
        {

            ViewBag.url = "Address";
            if (String.IsNullOrWhiteSpace(id))
            {
                return View();
            }
            else
            {
                if(id == "Add")
                {
                    using(WebStore.Models.webstoreEntities db = new WebStore.Models.webstoreEntities())
                    {
                        var viewModel = new BindSelect();

                        viewModel.countries = db.tblCountry.Select(x=> x).ToList();

                        viewModel.regions = db.tblRegiones.Select(x=>x).ToList();

                        viewModel.comunes = db.tblComunas.Where(x => x.refProvincia == 1).ToList();

                        return View("addAddress", viewModel);
                    }
                }
                else if (id == "Edit")
                {
                    return View("editAddress");
                }
                else
                {
                    return View();
                }
            }
        }

        public JsonResult Provinces(int id)
        {

            using (webstoreEntities db = new webstoreEntities())
            { 

                var provinces = db.tblProvincias.Where(x => x.refRegion == id).Select(x=> new { id = x.idProvincia, nombre = x.strNombre}).ToList();

                return Json(provinces, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Communes (int id)
        {
            using (webstoreEntities db = new webstoreEntities())
            {

                var communes = db.tblComunas.Where(x => x.refProvincia == id).Select(x => new { id = x.idComuna, nombre = x.strNombre }).ToList();

                return Json(communes, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Settings()
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                string email = Session["email"].ToString();
                int id = Int32.Parse(Session["id"].ToString());
                var user = db.tblUsers.Where(x => x.strEmail == email && x.idUser == id).FirstOrDefault();
                var address = (from a in db.tblAddresses
                               join b in db.tblRegiones
                               on a.refRegion equals b.idRegion
                               join c in db.tblProvincias
                               on a.refProvince equals c.idProvincia
                               join d in db.tblComunas
                               on a.refComuna equals d.idComuna
                               join e in db.tblUsers
                               on a.refuser equals e.idUser
                               where a.refuser == user.idUser && a.boolDefault == true
                               select new Addresses
                               {
                                   Id = a.idAddress,
                                   Country = a.strCountry,
                                   RegionId = b.idRegion,
                                   Region = b.strNombre,
                                   ProvinceId = c.idProvincia,
                                   Province = c.strNombre,
                                   ComunaId = d.idComuna,
                                   Comuna = d.strNombre,
                                   City = a.strCity,
                                   Address = a.strAddress,
                                   Poste = a.intPostalCode,
                                   Type = a.strType
                               }).FirstOrDefault();
                
                BindingAddress ViewModel = new BindingAddress()
                {
                    Regiones = db.tblRegiones.Select(x => x).ToList(),
                    Provinces = db.tblProvincias.Select(x => x).ToList(),
                    Comunes = db.tblComunas.Select(x => x).ToList(),
                    Addresses = address,
                    User = user
                };
                return View(ViewModel);
            }
        }

        public ActionResult Password()
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                int id = Int32.Parse(Session["id"].ToString());
                var user = db.tblUsers.Where(x => x.idUser == id).FirstOrDefault();
                if (user != null)
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
                            ViewBag.message = Json(new { status = "OK", title = "Código", responseText = "Se ha generado un código automáticamente y fue enviado a su dirección de email." });
                            return View();
                        }
                        catch (Exception ex)
                        {
                            ViewBag.message = Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.message = Json(new { status = "error", title = "Ups...!", responseText = email });
                        return View();
                    }

                }
                else
                {
                    ViewBag.message = Json(new { status = "warning", title = "Email", responseText = "Email no existe en nuestra base de datos." });
                    return View();
                }
            }
        }

        public ActionResult WishList()
        {

            ViewBag.url = "WishList";

            return View();
        }

        public ActionResult Tickets(int id = 0 )
        {

            ViewBag.url = "Tickets";

            if (id == 0)
            {
                return View();
            }
            else
            {
                ViewBag.Title = "Ticket ID:" + id;
                return View("detailsTickets");
            }
        }

        public JsonResult UpdateUserInfo(UserInfoUpdate user)
        {
            using (webstoreEntities db = new webstoreEntities())
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                var userInfo = db.tblUsers.Where(x => x.idUser == UserId).FirstOrDefault();
                var addressInfo = db.tblAddresses.Where(x => x.idAddress == user.Id && x.boolDefault == true).FirstOrDefault();
                userInfo.strNames = user.Name;
                userInfo.strLastNames = user.LastName;
                userInfo.intId = user.UserId;
                userInfo.intPhone = user.UserPhone;
                if(addressInfo != null)
                {
                    addressInfo.refRegion = user.Region;
                    addressInfo.refProvince = user.Province;
                    addressInfo.refComuna = user.Comuna;
                    addressInfo.strCity = user.City;
                    addressInfo.strAddress = user.Address;
                    addressInfo.strType = user.Type;
                    addressInfo.intPostalCode = user.PosteCode;
                    try
                    {
                        db.SaveChanges();
                        return Json(new { status = "OK", title = "¡Actualización exitosa!", responseText = "La actualización de sus tados fue realizada." }, JsonRequestBehavior.AllowGet);
                    }catch(Exception ex)
                    {
                        return Json(new { status = "error", title = "¡Ups..!", responseText = ex.ToString()}, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    tblAddresses NewAddress = new tblAddresses()
                    {
                        refRegion = user.Region,
                        refProvince = user.Province,
                        refComuna = user.Comuna,
                        strCity = user.City,
                        strAddress = user.Address,
                        strType = user.Type,
                        intPostalCode = user.PosteCode,
                        boolDefault = true,
                        refuser = UserId
                    };
                    try
                    {
                        db.tblAddresses.Add(NewAddress);
                        db.SaveChanges();
                        return Json(new { status = "OK", title = "¡Actualización exitosa!", responseText = "La actualización de sus tados fue realizada." }, JsonRequestBehavior.AllowGet);
                    }
                    catch(Exception ex)
                    {
                        return Json(new { status = "error", title = "¡Ups..!", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        private String SendValidationCode(string Email, string r)
        {
            var smtp = Configuration.GetSmtp();
            var toEmail = new MailAddress(Email);
            var divice = HttpContext.Request.Browser.IsMobileDevice;
            string fromDivice = divice ? "Móvil" : "Escritorio";
            string imgDivice = divice ? "/Content/img/bg/mobile.png" : "/Content/img/bg/desktop.png";
            string subject = "Solicitud para cambio de contraseña";
            string body = $@"<br/><br/>Hemos recibido una solicitud para cambiar su contraseña.
                            <br/><br/> Desde un dispositivo: <strong>{fromDivice}</strong>&nbsp;&nbsp;&nbsp;<img src='{imgDivice}' style='width: 25px; max-width: 25px;'/>
                            <br/> Hora: {DateTime.Now}
                            <br/> Ubicación: {Function.GetUserIp()}
                            <br/><br/>Se ha generado un código de verificación automático con un tiempo de expiración de 30 minutos.
                            <br/>Debe ingresar este código en la casilla de nuestra página web y proseguir con los pasos.
                            <br/>Su codigo de validación es:
                            <br/><br/><strong>{r}</strong>";
            try
            {
                smtp.Send(Function.GenerateEmail(toEmail, subject, body));
                return "Sent";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}