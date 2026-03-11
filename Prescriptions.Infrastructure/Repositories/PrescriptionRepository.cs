using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Prescriptions.Application.Repositories;
using Prescriptions.Domain.Entities;
using Prescriptions.Infrastructure.Persistence;

namespace Prescriptions.Infrastructure.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly PrescriptionsDbContext _context;

        public PrescriptionRepository(PrescriptionsDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Prescription> GetAll()
        {
            return _context.Prescriptions.ToList();
        }

        public Prescription GetById(Guid id)
        {
            return _context.Prescriptions.FirstOrDefault(x => x.Id == id);
        }

        public Prescription GetByCode(string code)
        {
            return _context.Prescriptions.FirstOrDefault(x => x.Code == code);
        }

        public IEnumerable<Prescription> GetByPatientId(Guid patientId)
        {
            return _context.Prescriptions
                .Where(x => x.PatientId == patientId)
                .ToList();
        }

        public void Add(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
        }

        public void Update(Prescription prescription)
        {
            var local = _context.Prescriptions.Local.FirstOrDefault(x => x.Id == prescription.Id);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(prescription).State = EntityState.Modified;
        }

        public void Delete(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
