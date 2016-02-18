using Suma2Lealtad.Filters;
using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

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

    }
}