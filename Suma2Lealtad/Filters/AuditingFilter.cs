using Suma2Lealtad.Models;
using Suma2Lealtad.Modules;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Suma2Lealtad.Filters
{
    public class AuditingFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["login"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    action = "SessionExpired",
                    controller = "Error",
                    area = ""
                }));
            }
            else
            {
                using (LealtadEntities db = new LealtadEntities())
                {
                    db.Database.Connection.ConnectionString = AppModule.ConnectionString("SumaLealtad");
                    int idlog;
                    if (db.Auditings.Count() > 0)
                    {
                        idlog = db.Auditings.Max(x => x.id) + 1;
                    }
                    else
                    {
                        idlog = 1;
                    }
                    Auditing log = new Auditing()
                    {
                        id = idlog,
                        objectid = 1,
                        operationsid = 1,
                        userid = (int)HttpContext.Current.Session["userid"],
                        creationdate = filterContext.HttpContext.Timestamp,
                        originaldata = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                        changedata = filterContext.ActionDescriptor.ActionName,
                        ipaddress = filterContext.HttpContext.Request.UserHostAddress
                    };
                    db.Auditings.Add(log);
                    db.SaveChanges();
                }
            }
            this.OnActionExecuting(filterContext);
        }

    }
}