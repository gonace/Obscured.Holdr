using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Obscured.Holdr.Service;
using Obscured.Holdr.Web.Filter;

namespace Obscured.Holdr.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "CategoryRoute", // Route name
                "{category}/{width}/{height}/{image}", // URL with parameters
                new { controller = "Home", action = "Generate", category = UrlParameter.Optional, width = UrlParameter.Optional, height = UrlParameter.Optional, image = UrlParameter.Optional },
                new { category = @"^[a-zA-Z]+$", width = @"^[a-zA-Z0-9]+$", height = @"^[a-zA-Z0-9]+$" }// Parameter defaults
            );

            routes.MapRoute(
                "ImageRoute", // Route name
                "{width}/{height}/{image}", // URL with parameters
                new { controller = "Home", action = "Generate", width = UrlParameter.Optional, height = UrlParameter.Optional, image = UrlParameter.Optional},
                new { width = @"^[a-zA-Z0-9]+$", height = @"^[a-zA-Z0-9]+$" }// Parameter defaults
            );

            routes.MapRoute(
                "DefaultRoute", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            AutoFacInitializer.Initialize();
        }

        public static class AutoFacInitializer
        {
            public static void Initialize()
            {
                var builder = new ContainerBuilder();
                builder.RegisterControllers(typeof(MvcApplication).Assembly);
                //builder.Register(c => new CustomAuthorizeAttribute(connectionString)).As<ICityControllerUnitOfWork>().InstancePerDependency();
                //builder.Register(c => new ImageService)).As<IImageService>().InstancePerDependency();
                builder.Register(c => new CustomAuthorizeAttribute()).As<AuthorizeAttribute>().InstancePerDependency();
                var container = builder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            }
        }
    }
}