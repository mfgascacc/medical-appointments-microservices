using People.Api.App_Start;
using System.Web;
using System.Web.Http;
using People.Api.DependencyInjection;
using People.Application.Repositories;
using People.Infrastructure.Persistence;
using People.Infrastructure.Repositories;

namespace People.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var resolver = new SimpleDependencyResolver();
            resolver.Register<IPersonRepository>(() => new PersonRepository(new PeopleDbContext()));
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}
