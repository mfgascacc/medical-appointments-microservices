using System;
using System.Collections.Generic;
using Prescriptions.Domain.Entities;

namespace Prescriptions.Application.Repositories
{
    public interface IPrescriptionRepository
    {
        IEnumerable<Prescription> GetAll();
        Prescription GetById(Guid id);
        Prescription GetByCode(string code);
        IEnumerable<Prescription> GetByPatientId(Guid patientId);
        void Add(Prescription prescription);
        void Update(Prescription prescription);
        void Delete(Prescription prescription);
        int SaveChanges();
    }
}
