using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Linq;

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
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel Model)
        {
            AppSession app = new AppSession();
            if (app.Login(Model.UserName, Model.Password))
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
            return RedirectToAction("Login");
        }

        [AuditingFilter]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Abandon();
            HttpContext.User = null;
            return RedirectToAction("Login");
        }

        public ActionResult Filter()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Filter(string login)
        {
            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                User user = (from u in db.Users
                             where u.login.Equals(login)
                             select u).FirstOrDefault();
                if (user != null)
                {
                    return View("CambiarPassword", user);
                }
                else
                {
                    ViewModel viewmodel = new ViewModel();
                    viewmodel.Title = "Usuario / Cambio de Contraseña";
                    viewmodel.Message = "Contraseña actualizada correctamente.";
                    viewmodel.ControllerName = "Home";
                    viewmodel.ActionName = "Login";
                    return RedirectToAction("GenericView", viewmodel);
                }
            }
        }

        [HttpPost]
        public ActionResult CambiarPassword(string login, string password)
        {
            ViewModel viewmodel = new ViewModel();

            using (LealtadEntities db = new LealtadEntities())
            {
                db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                User user = (from u in db.Users
                             where u.login.Equals(login)
                             select u).FirstOrDefault();
                if (user != null)
                {
                    user.passw = password;
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
                    viewmodel.ActionName = "Filter";
                }
                return RedirectToAction("GenericView", viewmodel);
            }
        }

        public ActionResult GenericView(ViewModel viewmodel)
        {
            return View(viewmodel);
        }

    }
}