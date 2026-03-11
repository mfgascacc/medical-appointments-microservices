using System;
using Appointments.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Domain.Entities
{
    public class Appointment
    {
        protected Appointment()
        {
            // Required by EF6 for materialization.
        }

        public Guid Id { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid PatientId { get; private set; }
        public DateTime ScheduledAt { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public Appointment(Guid id, Guid doctorId, Guid patientId, DateTime scheduledAt)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id inválido.");
            if (doctorId == Guid.Empty) throw new ArgumentException("DoctorId inválido.");
            if (patientId == Guid.Empty) throw new ArgumentException("PatientId inválido.");
            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            ScheduledAt = scheduledAt;
            Status = AppointmentStatus.Pending;
        }
        public void Start()
        {
            if (Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Solo se puede iniciar una cita en estado Pending.");
            Status = AppointmentStatus.InProgress;
        }
        public void Finish()
        {
            if (Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException("Solo se puede finalizar una cita en estado InProgress.");
            Status = AppointmentStatus.Finished;
        }
    }
}
