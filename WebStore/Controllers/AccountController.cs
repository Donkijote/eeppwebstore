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
using System.IO;

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

        public ActionResult Address(string id = null, int? idp = null)
        {

            if (String.IsNullOrWhiteSpace(id))
            {
                using(webstoreEntities db = new webstoreEntities())
                {
                    int userId = Int32.Parse(Session["id"].ToString());
                    var Address = (from a in db.tblAddresses
                                   join b in db.tblRegiones
                                   on a.refRegion equals b.idRegion
                                   join c in db.tblProvincias
                                   on a.refProvince equals c.idProvincia
                                   join d in db.tblComunas
                                   on a.refComuna equals d.idComuna
                                   join e in db.tblUsers
                                   on a.refuser equals e.idUser
                                   where a.refuser == userId
                                   select new Addresses
                                   {
                                       Id = a.idAddress,
                                       Region = b.strNombre,
                                       Province = c.strNombre,
                                       Comuna = d.strNombre,
                                       City = a.strCity,
                                       Address = a.strAddress,
                                       AddressTwo = a.strAddressTwo,
                                       Poste = a.intPostalCode,
                                       Type = a.strType,
                                       Default = a.boolDefault ? (bool)a.boolDefault : false,
                                       Third = a.boolThird ? (bool)a.boolThird : false,
                                       Tlf = e.intPhone
                                   }).ToList();
                    foreach(var i in Address)
                    {
                        if (i.Third)
                        {
                            var f = db.tblAddressesDet.Where(x => x.refAddress == i.Id).FirstOrDefault();
                            i.Names = f.strName;
                            i.LastNames = f.strLastName;
                            i.Phone = (int)f.intPhone;
                        }
                    }
                    return View(Address);
                }                
            }
            else
            {
                if(id == "Add")
                {
                    using(webstoreEntities db = new webstoreEntities())
                    {
                        var viewModel = new BindSelect
                        {
                            countries = db.tblCountry.Select(x => x).ToList(),

                            regions = db.tblRegiones.Select(x => x).ToList(),

                            comunes = db.tblComunas.Where(x => x.refProvincia == 1).ToList()
                        };

                        return View("addAddress", viewModel);
                    }
                }
                else if (id == "Edit")
                {
                    if(idp != null)
                    {
                        using (webstoreEntities db = new webstoreEntities())
                        {
                            string email = Session["email"].ToString();
                            int idUser = Int32.Parse(Session["id"].ToString());
                            var user = db.tblUsers.Where(x => x.strEmail == email && x.idUser == idUser).FirstOrDefault();
                            var address = GetAddresses(db, user.idUser).Where(x => x.Id == idp).FirstOrDefault();
                            var addressDet = db.tblAddressesDet.Where(x => x.refAddress == address.Id).FirstOrDefault();

                            BindingAddress ViewModel = new BindingAddress()
                            {
                                Regiones = db.tblRegiones.Select(x => x).ToList(),
                                Provinces = db.tblProvincias.Select(x => x).ToList(),
                                Comunes = db.tblComunas.Select(x => x).ToList(),
                                Addresses = address,
                                AddressesDet = addressDet,
                                User = user,

                            };
                            return View("editAddress", ViewModel);
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddNewAddress(AddNewAddress newAddress)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                tblAddresses addresses = new tblAddresses
                {
                    refuser = UserId,
                    refRegion = newAddress.States,
                    refProvince = newAddress.Provincia,
                    refComuna = newAddress.Comuna,
                    intPostalCode = newAddress.PosteCode,
                    strCity = newAddress.City,
                    strAddress = newAddress.Address,
                    strAddressTwo = newAddress.AddressTwo,
                    strType = newAddress.Type
                };

                var ifAny = db.tblAddresses.Where(x => x.refuser == UserId).ToList();
                if (ifAny.Any())
                {
                    addresses.boolDefault = false;
                }
                else
                {
                    addresses.boolDefault = true;
                }

                try
                {
                    db.tblAddresses.Add(addresses);
                    db.SaveChanges();
                    return Json(new { status = "OK", title = "Dirección añadida", responseText = "La dirección ha sido añadida correctamente." }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult SetDefaultAddress(int id)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                List<tblAddresses> addresses = db.tblAddresses.Where(x => x.refuser == UserId).Select(x => x).ToList();

                foreach(var i in addresses)
                {
                    if(i.idAddress == id)
                    {
                        i.boolDefault = true;
                    }
                    else
                    {
                        i.boolDefault = false;
                    }
                }

                try
                {
                    db.SaveChanges();
                    return Json(new { status = "OK" });
                }catch(Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditAddress(AddNewAddress newAddress)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                tblAddresses Dir = db.tblAddresses.Where(x => x.idAddress == newAddress.PreviousId).FirstOrDefault();

                Dir.refRegion = newAddress.States;
                Dir.refProvince = newAddress.Provincia;
                Dir.refComuna = newAddress.Comuna;
                Dir.strAddress = newAddress.Address;
                Dir.strAddressTwo = newAddress.AddressTwo;
                Dir.strCity = newAddress.City;
                Dir.intPostalCode = newAddress.PosteCode;
                Dir.strType = newAddress.Type;
                try
                {
                    db.SaveChanges();
                    return Json(new { status = "OK", title = "Dirección actualizada", responseText = "La dirección fue actualizada correctamente." });
                }catch(Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                }
            }
        }

        [HttpPost]
        public JsonResult SetDeleteAddress(int id)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                try
                {
                    var Dir = new tblAddresses { idAddress = id };
                    db.tblAddresses.Attach(Dir);
                    db.tblAddresses.Remove(Dir);
                    db.SaveChanges();
                    return Json(new { status = "OK" });
                }catch(Exception ex)
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                }
            }
        }

        [HttpPost]
        public JsonResult ChangeAvatarImg()
        {
            if(Request.Files["AvatarFile"].ContentLength > 0)
            {
                string FileExtension = Path.GetExtension(Request.Files["AvatarFile"].FileName);
                if(FileExtension == ".png" || FileExtension == ".jpg")
                {
                    try
                    {
                        string file = Server.MapPath("~/Content/img/avatars/img/") + Session["id"].ToString() + ".jpg";
                        string fileLocation = fileLocation = Server.MapPath("~/Content/img/avatars/img/") + Session["id"].ToString() + FileExtension;
                        if (System.IO.File.Exists(file))
                        {
                            System.IO.File.Delete(file);
                        }
                        else
                        {
                            file = Server.MapPath("~/Content/img/avatars/img/") + Session["id"].ToString() + ".png";
                            if (System.IO.File.Exists(file))
                            {
                                System.IO.File.Delete(file);
                            }
                        }

                        string resp = AddAvatar(Int32.Parse(Session["id"].ToString()), Session["id"].ToString() + Path.GetExtension(Request.Files["AvatarFile"].FileName), "img");
                        if(resp == "ok")
                        {
                            Request.Files["AvatarFile"].SaveAs(fileLocation);
                            Session["img"] = Session["id"].ToString() + FileExtension;
                            Session["imgType"] = "img";
                            return Json(new { status = "OK"});
                        }
                        else {
                            return Json(new { status = "error", title = "Ups...!", responseText = resp });
                        }
                        
                    }catch(Exception ex)
                    {
                        return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
                    }
                }
                else {
                    return Json(new { status = "warning", responsetext = "Archivo debe ser tipo: png, jpg" });
                }
            }
            else {
                return Json(new { status = "warning", responsetext = "No seleccionó ningún archivo" });
            }
        }
        [HttpPost]
        public JsonResult RemoveAvatarImg()
        {
            try
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                string file = Server.MapPath("~/Content/img/avatars/img/") + Session["id"].ToString() + ".jpg";
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                    Session.Remove("img");
                    Session.Remove("imgType");
                }
                else
                {
                    file = Server.MapPath("~/Content/img/avatars/img/") + Session["id"].ToString() + ".png";
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                        Session.Remove("img");
                        Session.Remove("imgType");
                    }
                }
                string remove = RemoveAvatar(UserId, "img");
                if(remove != "ok" && remove != "profile")
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = remove });
                }
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
            }

        }
        [HttpPost]
        public JsonResult ChangeAvatarIcon(string Icon, string Type)
        {
            string resp = AddAvatar(Int32.Parse(Session["id"].ToString()), Icon, Type);
            if (resp == "ok")
            {
                Session["img"] = Icon;
                Session["imgType"] = Type;
                return Json(new { status = "OK" });
            }
            else
            {
                return Json(new { status = "error", title = "Ups...!", responseText = resp });
            }
        }
        [HttpPost]
        public JsonResult RemoveAvatarIcon()
        {
            try
            {
                int UserId = Int32.Parse(Session["id"].ToString());
                Session.Remove("img");
                Session.Remove("imgType");
                string remove = RemoveAvatar(UserId, "icon");
                if (remove != "ok" && remove != "profile")
                {
                    return Json(new { status = "error", title = "Ups...!", responseText = remove });
                }
                return Json(new { status = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", title = "Ups...!", responseText = ex.ToString() });
            }

        }

        public ActionResult Settings()
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                string email = Session["email"].ToString();
                int id = Int32.Parse(Session["id"].ToString());
                var user = db.tblUsers.Where(x => x.strEmail == email && x.idUser == id).FirstOrDefault();
                var address = GetAddresses(db, user.idUser).Where(x => x.Default == true).FirstOrDefault();
                
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PasswordChange(string ValidationCode, string Password, string ConfirmPassword)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                int id = Int32.Parse(Session["id"].ToString());
                var user = db.tblUsers.Where(x => x.idUser == id).FirstOrDefault();
                if (user != null)
                {
                    if(user.strRecoveryCode == ValidationCode)
                    {
                        if (user.timeRecoveryCode.Value.AddMinutes(30) > DateTime.Now)
                        {
                            if (Password == ConfirmPassword)
                            {
                                user.strPassword = Crypto.Hash(Password);
                                try
                                {
                                    db.SaveChanges();
                                    return Json(new { status = "OK", title = "Contraseña", responseText = "Su contraseña fue actualizada con éxito." }, JsonRequestBehavior.AllowGet);
                                }
                                catch (Exception ex)
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
                            return Json(new { status = "warning", title = "Expiración", responseText = "El código de validación ha expirado." }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else{
                        return Json(new { status = "warning", title = "Código", responseText = "El código de validación no es válido." }, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else
                {
                    return Json(new { status = "error", title = "Código", responseText = "Código de validación incorrecto." }, JsonRequestBehavior.AllowGet);
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
                    addressInfo.boolThird = false;
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
                        boolThird = false,
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
            string imgDivice = divice ? "http:eeppwebstore.ddns.net/Content/img/bg/mobile.png" : "http:eeppwebstore.ddns.net/Content/img/bg/desktop.png";
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

        private List<Addresses> GetAddresses(webstoreEntities db, int id)
        {
            return (from a in db.tblAddresses
             join b in db.tblRegiones
             on a.refRegion equals b.idRegion
             join c in db.tblProvincias
             on a.refProvince equals c.idProvincia
             join d in db.tblComunas
             on a.refComuna equals d.idComuna
             join e in db.tblUsers
             on a.refuser equals e.idUser
             where a.refuser == id
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
                 AddressTwo = a.strAddressTwo,
                 Poste = a.intPostalCode,
                 Type = a.strType,
                 Default = (bool)a.boolDefault,
                 Third = a.boolThird ? (bool)a.boolThird : false
             }).ToList();
        }

        private string AddAvatar(int user, string file, string type)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                try
                {
                    var avatar = db.tblAvatars.Where(x => x.refUser == user).FirstOrDefault();
                    if (avatar != null)
                    {
                        avatar.strAvatarType = type;
                        avatar.strAvatarName = file;
                    }
                    else
                    {
                        var newAvatar = new tblAvatars
                        {
                            refUser = user,
                            strAvatarName = file,
                            strAvatarType = type
                        };
                        db.tblAvatars.Add(newAvatar);
                    }
                    db.SaveChanges();
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.ToString();
                }
            }
        }
        private string RemoveAvatar(int user, string type)
        {
            using(webstoreEntities db = new webstoreEntities())
            {
                var avatar = db.tblAvatars.Where(x => x.refUser == user && x.strAvatarType == type).FirstOrDefault();

                if(avatar != null)
                {
                    try
                    {
                        db.tblAvatars.Remove(avatar);
                        db.SaveChanges();
                        return "ok";
                    }catch(Exception ex)
                    {
                        return ex.ToString();
                    }
                }
                return "profile";
            }
        }
    }
}