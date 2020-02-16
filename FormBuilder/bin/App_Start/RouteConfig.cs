using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FormBuilder
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //** BEGIN FORM BUILDER ROUTES **//

            // download file
            routes.MapRoute(
               "form-download-file",
               "forms/file/download/{valueId}",
               new { controller = "forms", action = "GetFileFromDisk", valueId = UrlParameter.Optional }
            );


            // register
            routes.MapRoute(
               "form-edit",
               "forms/edit/{id}",
               new { controller = "forms", action = "edit", Id = UrlParameter.Optional },
               new { Id = @"\d+" }
            );

            // register
            routes.MapRoute(
               "form-preview",
               "forms/preview/{id}",
               new { controller = "forms", action = "preview", Id = UrlParameter.Optional },
               new { Id = @"\d+" }
            );

            // register
            routes.MapRoute(
               "form-register",
               "forms/register/{id}/{embed}",
               new { controller = "forms", action = "register", Id = UrlParameter.Optional, embed = UrlParameter.Optional },
               new { Id = @"\d+" }
            );

            // create form
            routes.MapRoute(
               "form-create",
               "forms/create",
               new { controller = "forms", action = "create" }
            );

            // form confirmation
            routes.MapRoute(
               "form-confirmation",
               "forms/confirmation/{id}/{embed}",
               new { controller = "forms", action = "formconfirmation", Id = UrlParameter.Optional, embed = UrlParameter.Optional },
               new { Id = @"\d+" }
            );

            // view form entries
            routes.MapRoute(
               "form-entries",
               "forms/entries/{formid}",
               new { controller = "forms", action = "ViewEntries", formId = UrlParameter.Optional },
               new { formId = @"\d+" }
            );

            // delete form
            routes.MapRoute(
               "form-delete",
               "forms/delete/{formid}",
               new { controller = "forms", action = "delete", formId = UrlParameter.Optional },
               new { formId = @"\d+" }
            );

            // forms home
            routes.MapRoute(
               "form-home",
               "forms/index",
               new { controller = "forms", action = "index" }
            );

            //** END FORM BUILDER ROUTES **//

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "forms", action = "index", id = UrlParameter.Optional }
            );

        }
    }
}
