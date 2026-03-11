using Prescriptions.Api.App_Start;
using Prescriptions.Api.DependencyInjection;
using Prescriptions.Application.Repositories;
using Prescriptions.Infrastructure.Persistence;
using Prescriptions.Infrastructure.Repositories;
using Prescriptions.Api.Messaging;
using System.Web;
using System.Web.Http;

namespace Prescriptions.Api
{
    public class WebApiApplication : HttpApplication
    {
        private static AppointmentFinishedConsumer _appointmentFinishedConsumer;

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var resolver = new SimpleDependencyResolver();
            resolver.Register<IPrescriptionRepository>(() => new PrescriptionRepository(new PrescriptionsDbContext()));
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            _appointmentFinishedConsumer = new AppointmentFinishedConsumer();
            _appointmentFinishedConsumer.Start();
        }

        protected void Application_End()
        {
            _appointmentFinishedConsumer?.Dispose();
        }
    }
}