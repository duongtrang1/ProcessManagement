﻿using System.Web.Mvc;

namespace ProcessManagement.Areas.API
{
    public class APIAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "API";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "API_default",
                "API/{controller}/{action}",
                namespaces: new[] { "ProcessManagement.Areas.API.Controllers" }
            );
        }
    }
}