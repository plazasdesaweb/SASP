using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using Suma2Lealtad.Models.ViewModels.Home;


namespace Suma2Lealtad.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {


        public ActionResult Login()
        {
            Session["titulo"] = "Administrador SUMA / PREPAGO";
            Session["login"] = null;
            //PARA GUARDAR EL AMBIENTE DE CORRIDA CONFIGURADO EN EL WEBCONFIG
            Session["ambiente"] = ConfigurationManager.AppSettings["AMBIENTE"].ToString();
            //PARA GUARDAR LA VERSION CONFIGURADO EN EL WEBCONFIG
            Session["version"] = ConfigurationManager.AppSettings["VERSION"].ToString();
            LoginViewModel login = new LoginViewModel();
            return View(login);
        }

        [HttpPost]
        public ActionResult Index(LoginViewModel login, string Command)
        {
            //secion de comando iniciar sesión 
            if (Command == "Iniciar Sesión")
            {
                if (ModelState.IsValid)
                {
                    AppSession app = new AppSession();
                    if (app.Login(login.UserName, login.Password))
                    {
                        if (app.UserType == "Prepago")
                        {
                            Session["titulo"] = "Administrador PREPAGO";
                        }
                        else
                        {
                            Session["titulo"] = "Administrador SUMA";
                        }
                        //para guardar el RoleLevel
                        Session["RoleLevel"] = app.RoleLevel;
                        Session["login"] = app.UserLogin;
                        Session["username"] = app.UserName;
                        Session["userid"] = app.UserID;
                        Session["type"] = app.UserType;
                        Session["menu"] = app.MenuList;
                        Session["appdate"] = app.AppDate;
                        ViewBag.AppDate = app.AppDate;
                        ViewBag.Menu = app.MenuList;
                        return View();
                    }
                    else
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("UserName", "La combinación Usuario/Contraseña es inválida.");
                        return View("Login", login);
                    }
                }
                else
                {
                    return View("Login", login);
                }
            }
            //secion de comando olvidó contraseña
            else
            {
                if (login.UserName == null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("UserName", "El campo Usuario es obligatorio.");
                    return View("Login", login);
                }
                using (LealtadEntities db = new LealtadEntities())
                {
                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                    User user = (from u in db.Users
                                 where u.login.Equals(login.UserName)
                                 select u).FirstOrDefault();
                    if (user == null)
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("UserName", "El Usuario suministrado no existe. Verifique.");
                        return View("Login", login);
                    }
                }
                CambiarPasswordViewModel cambiarpassword = new CambiarPasswordViewModel()
                {
                    UserName = login.UserName
                };
                return View("CambiarPassword", cambiarpassword);
            }
        }

        [AuditingFilter]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.User = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult CambiarPassword(CambiarPasswordViewModel cambiarpassword)
        {
            if (cambiarpassword.NewPassword1 == null || cambiarpassword.NewPassword2 == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("NewPassword1", "Debe ingresar la Nueva Contraseña y su confirmación");
            }
            if (ModelState.IsValid)
            {
                if (cambiarpassword.NewPassword1 != cambiarpassword.NewPassword2)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("NewPassword1", "La Nueva Contraseña y su confirmación no coinciden");
                    return View(cambiarpassword);
                }
                using (LealtadEntities db = new LealtadEntities())
                {
                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                    User user = (from u in db.Users
                                 where u.login.Equals(cambiarpassword.UserName)
                                 select u).FirstOrDefault();
                    ViewModel viewmodel = new ViewModel();
                    if (user != null)
                    {
                        user.passw = cambiarpassword.NewPassword1;
                        db.SaveChanges();
                        viewmodel.Title = "Usuario / Cambio de Contraseña";
                        viewmodel.Message = "Contraseña actualizada correctamente.";
                        viewmodel.ControllerName = "Home";
                        viewmodel.ActionName = "Login";
                    }
                    else
                    {
                        viewmodel.Title = "Usuario / Cambio de Contraseña";
                        viewmodel.Message = "No se pudo actulizar la contraseña correctamente.";
                        viewmodel.ControllerName = "Home";
                        viewmodel.ActionName = "Login";
                    }
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
            else
            {
                return View(cambiarpassword);
            }
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

    }
}