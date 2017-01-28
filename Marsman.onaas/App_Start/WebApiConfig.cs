using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Marsman.onaas
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "OhNoApi",
				routeTemplate: "{controller}/{activity}/{error}",
				defaults: new {  }, constraints: new { controller = "OhNo" }
			);

			config.Routes.MapHttpRoute(
				name: "OhYeahApi",
				routeTemplate: "{controller}",
				defaults: new {  }, constraints: new { controller = "OhYeah" }
			);

			config.Routes.MapHttpRoute(
				name: "TestApi",
				routeTemplate: "{controller}/{action}",
				defaults: new { }, constraints: new { controller = "Test" }
			);
		}
	}
}
