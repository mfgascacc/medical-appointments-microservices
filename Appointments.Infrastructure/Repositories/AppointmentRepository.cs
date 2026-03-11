using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Appointments.Application.Repositories;
using Appointments.Domain.Entities;
using Appointments.Infrastructure.Persistence;

namespace Appointments.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentsDbContext _context;

        public AppointmentRepository(AppointmentsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Appointment> GetAll()
        {
            return _context.Appointments.ToList();
        }

        public Appointment GetById(Guid id)
        {
            return _context.Appointments.FirstOrDefault(x => x.Id == id);
        }

        public bool ExistsDoctorConflict(Guid doctorId, DateTime scheduledAt, Guid? excludeId = null)
        {
            var query = _context.Appointments.Where(x => x.DoctorId == doctorId && x.ScheduledAt == scheduledAt);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return query.Any();
        }

        public bool ExistsPatientConflict(Guid patientId, DateTime scheduledAt, Guid? excludeId = null)
        {
            var query = _context.Appointments.Where(x => x.PatientId == patientId && x.ScheduledAt == scheduledAt);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return query.Any();
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public void Update(Appointment appointment)
        {
            var local = _context.Appointments.Local.FirstOrDefault(x => x.Id == appointment.Id);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(appointment).State = EntityState.Modified;
        }

        public void Delete(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
