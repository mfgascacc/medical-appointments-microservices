using System;
using System.Net;
using System.Linq;
using System.Web.Http;
using Appointments.Application.Repositories;
using Appointments.Domain.Entities;
using Appointments.Domain.Enums;
using Appointments.Api.Clients;
using Appointments.Api.Messaging;
using Appointments.Api.Security;

namespace Appointments.Api.Controllers
{
    [RoutePrefix("api/appointments")]
    [JwtAuthorize]
    public class AppointmentsController : ApiController
    {
        private readonly IAppointmentRepository _appointmentRepository;

        private readonly IPeopleClient _peopleClient;
        private readonly IAppointmentEventPublisher _eventPublisher;

        public AppointmentsController(
            IAppointmentRepository appointmentRepository,
            IPeopleClient peopleClient,
            IAppointmentEventPublisher eventPublisher)
        {
            _appointmentRepository = appointmentRepository;
            _peopleClient = peopleClient;
            _eventPublisher = eventPublisher;
        }
     

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var appointments = _appointmentRepository.GetAll()
                .Select(MapToResponse)
                .ToList();

            return Ok(appointments);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(appointment));
        }



        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] UpsertAppointmentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            var doctor = _peopleClient.GetPersonById(request.DoctorId);
            if (doctor == null || doctor.Type != 1)
            {
                return BadRequest("DoctorId inv?lido o no corresponde a un doctor.");
            }

            var patient = _peopleClient.GetPersonById(request.PatientId);
            if (patient == null || patient.Type != 2)
            {
                return BadRequest("PatientId inv?lido o no corresponde a un paciente.");
            }


            if (request.DoctorId == request.PatientId)
            {
                return BadRequest("DoctorId y PatientId no pueden ser iguales.");
            }

            if (_appointmentRepository.ExistsDoctorConflict(request.DoctorId, request.ScheduledAt))
            {
                return Content(HttpStatusCode.Conflict, "El doctor ya tiene una cita en ese horario.");
            }

            if (_appointmentRepository.ExistsPatientConflict(request.PatientId, request.ScheduledAt))
            {
                return Content(HttpStatusCode.Conflict, "El paciente ya tiene una cita en ese horario.");
            }

            var appointment = BuildAppointment(Guid.NewGuid(), request);
            _appointmentRepository.Add(appointment);
            _appointmentRepository.SaveChanges();
            PublishIfFinished(appointment);

            return Created($"api/appointments/{appointment.Id}", MapToResponse(appointment));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] UpsertAppointmentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            if (request.DoctorId == request.PatientId)
            {
                return BadRequest("DoctorId y PatientId no pueden ser iguales.");
            }

            var existing = _appointmentRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (_appointmentRepository.ExistsDoctorConflict(request.DoctorId, request.ScheduledAt, id))
            {
                return Content(HttpStatusCode.Conflict, "El doctor ya tiene una cita en ese horario.");
            }

            if (_appointmentRepository.ExistsPatientConflict(request.PatientId, request.ScheduledAt, id))
            {
                return Content(HttpStatusCode.Conflict, "El paciente ya tiene una cita en ese horario.");
            }

            var updated = BuildAppointment(id, request);
            _appointmentRepository.Update(updated);
            _appointmentRepository.SaveChanges();
            var isFirstTimeFinished = existing.Status != AppointmentStatus.Finished &&
                                      updated.Status == AppointmentStatus.Finished;
            if (isFirstTimeFinished)
            {
                PublishIfFinished(updated);
            }

            return Ok(MapToResponse(updated));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            var appointment = _appointmentRepository.GetById(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _appointmentRepository.Delete(appointment);
            _appointmentRepository.SaveChanges();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        private static Appointment BuildAppointment(Guid id, UpsertAppointmentRequest request)
        {
            var appointment = new Appointment(id, request.DoctorId, request.PatientId, request.ScheduledAt);

            var requestedStatus = (AppointmentStatus)request.Status;
            if (requestedStatus == AppointmentStatus.InProgress)
            {
                appointment.Start();
            }
            else if (requestedStatus == AppointmentStatus.Finished)
            {
                appointment.Start();
                appointment.Finish();
            }

            return appointment;
        }

        private static AppointmentResponse MapToResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                ScheduledAt = appointment.ScheduledAt,
                Status = (int)appointment.Status
            };
        }

        private void PublishIfFinished(Appointment appointment)
        {
            if (appointment.Status == AppointmentStatus.Finished)
            {
                _eventPublisher.PublishAppointmentFinished(appointment);
            }
        }
    }

    public class UpsertAppointmentRequest
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Status { get; set; }
    }

    public class AppointmentResponse
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int Status { get; set; }
    }
}
