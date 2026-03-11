using Appointments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application.Repositories
{
    public interface IAppointmentRepository
    {
        IEnumerable<Appointment> GetAll();
        Appointment GetById(Guid id);
        bool ExistsDoctorConflict(Guid doctorId, DateTime scheduledAt, Guid? excludeId = null);
        bool ExistsPatientConflict(Guid patientId, DateTime scheduledAt, Guid? excludeId = null);
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(Appointment appointment);
        int SaveChanges();
    }
}
