using Suma2Lealtad.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Suma2Lealtad
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http//go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            Console.Write("Application_End");
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            Console.Write("Application_Error");
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
            Console.Write("Application_BeginRequest");
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            Console.Write("Application_EndRequest");
        }

        public void Session_OnStart()
        {
            Console.Write("Session_OnStart");
        }

        public void Session_OnEnd()
        {
            Session.Abandon();
            Console.Write("Session_OnEnd");
        }

    }
}