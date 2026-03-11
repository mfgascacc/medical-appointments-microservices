using System;
using Prescriptions.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prescriptions.Domain.Entities
{
    public class Prescription
    {
        protected Prescription()
        {
            // Required by EF6 for materialization.
        }

        public Guid Id { get; private set; }
        public string Code { get; private set; }
        public Guid PatientId { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public PrescriptionStatus Status { get; private set; }
        public Prescription(Guid id, string code, Guid patientId, DateTime issuedAt)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id inválido.");
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Code requerido.");
            if (patientId == Guid.Empty) throw new ArgumentException("PatientId inválido.");
            Id = id;
            Code = code.Trim();
            PatientId = patientId;
            IssuedAt = issuedAt;
            Status = PrescriptionStatus.Active;
        }
        public void MarkDelivered()
        {
            if (Status != PrescriptionStatus.Active)
                throw new InvalidOperationException("Solo recetas activas pueden pasar a Delivered.");
            Status = PrescriptionStatus.Delivered;
        }
        public void MarkExpired()
        {
            if (Status != PrescriptionStatus.Active)
                throw new InvalidOperationException("Solo recetas activas pueden pasar a Expired.");
            Status = PrescriptionStatus.Expired;
        }
    }
}
