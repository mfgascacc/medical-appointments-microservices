using System;
using System.Linq;
using System.Web.Http;
using Prescriptions.Api.Security;
using Prescriptions.Application.Repositories;
using Prescriptions.Domain.Entities;
using Prescriptions.Domain.Enums;

namespace Prescriptions.Api.Controllers
{
    [RoutePrefix("api/prescriptions")]
    [JwtAuthorize]
    public class PrescriptionsController : ApiController
    {
        private readonly IPrescriptionRepository _prescriptionRepository;

        public PrescriptionsController(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var prescriptions = _prescriptionRepository.GetAll()
                .Select(MapToResponse)
                .ToList();

            return Ok(prescriptions);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            var prescription = _prescriptionRepository.GetById(id);
            if (prescription == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(prescription));
        }

        [HttpGet]
        [Route("by-code/{code}")]
        public IHttpActionResult GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Code requerido.");
            }

            var prescription = _prescriptionRepository.GetByCode(code);
            if (prescription == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(prescription));
        }

        [HttpGet]
        [Route("by-patient/{patientId:guid}")]
        public IHttpActionResult GetByPatient(Guid patientId)
        {
            var prescriptions = _prescriptionRepository.GetByPatientId(patientId)
                .Select(MapToResponse)
                .ToList();

            return Ok(prescriptions);
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] UpsertPrescriptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            var prescription = BuildPrescription(Guid.NewGuid(), request);
            _prescriptionRepository.Add(prescription);
            _prescriptionRepository.SaveChanges();

            return Created($"api/prescriptions/{prescription.Id}", MapToResponse(prescription));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] UpsertPrescriptionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            var existing = _prescriptionRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            var updated = BuildPrescription(id, request);
            _prescriptionRepository.Update(updated);
            _prescriptionRepository.SaveChanges();

            return Ok(MapToResponse(updated));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            var prescription = _prescriptionRepository.GetById(id);
            if (prescription == null)
            {
                return NotFound();
            }

            _prescriptionRepository.Delete(prescription);
            _prescriptionRepository.SaveChanges();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        private static Prescription BuildPrescription(Guid id, UpsertPrescriptionRequest request)
        {
            var prescription = new Prescription(id, request.Code, request.PatientId, request.IssuedAt);

            var requestedStatus = (PrescriptionStatus)request.Status;
            if (requestedStatus == PrescriptionStatus.Delivered)
            {
                prescription.MarkDelivered();
            }
            else if (requestedStatus == PrescriptionStatus.Expired)
            {
                prescription.MarkExpired();
            }

            return prescription;
        }

        private static PrescriptionResponse MapToResponse(Prescription prescription)
        {
            return new PrescriptionResponse
            {
                Id = prescription.Id,
                Code = prescription.Code,
                PatientId = prescription.PatientId,
                IssuedAt = prescription.IssuedAt,
                Status = (int)prescription.Status
            };
        }
    }

    public class UpsertPrescriptionRequest
    {
        public string Code { get; set; }
        public Guid PatientId { get; set; }
        public DateTime IssuedAt { get; set; }
        public int Status { get; set; }
    }

    public class PrescriptionResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public Guid PatientId { get; set; }
        public DateTime IssuedAt { get; set; }
        public int Status { get; set; }
    }
}
