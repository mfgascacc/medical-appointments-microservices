using System.Web.Http;
using System.Web;
using Appointments.Api.App_Start;
using Appointments.Api.DependencyInjection;
using Appointments.Application.Repositories;
using Appointments.Infrastructure.Persistence;
using Appointments.Infrastructure.Repositories;
using Appointments.Api.Clients;
using Appointments.Api.Messaging;


namespace Appointments.Api
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var resolver = new SimpleDependencyResolver();
            resolver.Register<IAppointmentRepository>(() => new AppointmentRepository(new AppointmentsDbContext()));
            resolver.Register<IPeopleClient>(() => new PeopleClient());
            resolver.Register<IAppointmentEventPublisher>(() => new AppointmentEventPublisher());
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }
    }
}