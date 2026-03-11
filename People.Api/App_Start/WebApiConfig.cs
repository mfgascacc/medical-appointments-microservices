using System.Web.Http;
using Swashbuckle.Application;

namespace People.Api.App_Start
{
    public class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config
                .EnableSwagger(c => c.SingleApiVersion("v1", "People.Api"))
                .EnableSwaggerUi();
        }
    }
}
