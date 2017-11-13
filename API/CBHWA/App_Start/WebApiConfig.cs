using System.Web.Http;
using System.Web.Http.Cors;

namespace CBHWA
{
    public static class WebApiConfig
    {
        //    public static void Register(HttpConfiguration config)
        //    {
        //        // Web API configuration and services
        //        //var cors = new EnableCorsAttribute("*", "*", "*");
        //        //config.EnableCors();

        //        // Web API routes
        //        //config.MapHttpAttributeRoutes();

        //        config.Routes.MapHttpRoute(
        //            name: "DefaultApi",
        //            routeTemplate: "api/{controller}/{id}",
        //            defaults: new { id = RouteParameter.Optional }
        //        );

        //        // Quite los comentarios de la siguiente línea de código para habilitar la compatibilidad de consultas para las acciones con un tipo de valor devuelto IQueryable o IQueryable<T>.
        //        // Para evitar el procesamiento de consultas inesperadas o malintencionadas, use la configuración de validación en QueryableAttribute para validar las consultas entrantes.
        //        // Para obtener más información, visite http://go.microsoft.com/fwlink/?LinkId=279712.
        //        //config.EnableQuerySupport();

        //        // Para deshabilitar el seguimiento en la aplicación, incluya un comentario o quite la siguiente línea de código
        //        // Para obtener más información, consulte: http://www.asp.net/web-api
        //        //config.EnableSystemDiagnosticsTracing();
        //    }
        //}

        public static void Register(HttpConfiguration config)
        {
            EnableCrossSiteRequests(config);
            AddRoutes(config);
        }

        private static void AddRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }
    }
}
