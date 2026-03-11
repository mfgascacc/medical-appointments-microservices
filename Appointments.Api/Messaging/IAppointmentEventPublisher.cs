using Appointments.Domain.Entities;

namespace Appointments.Api.Messaging
{
    public interface IAppointmentEventPublisher
    {
        void PublishAppointmentFinished(Appointment appointment);
    }
}
