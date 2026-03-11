using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using People.Api.Security;
using People.Application.Repositories;
using People.Domain.Entities;
using People.Domain.Enums;

namespace People.Api.Controllers
{
    [RoutePrefix("api/people")]
    [JwtAuthorize]
    public class PeopleController : ApiController
    {
        private readonly IPersonRepository _personRepository;

        public PeopleController(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var people = _personRepository.GetAll()
                .Select(MapToResponse)
                .ToList();

            return Ok(people);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetById(Guid id)
        {
            var person = _personRepository.GetById(id);
            if (person == null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(person));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create([FromBody] UpsertPersonRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            var person = BuildPerson(Guid.NewGuid(), request);
            _personRepository.Add(person);
            _personRepository.SaveChanges();

            return Created($"api/people/{person.Id}", MapToResponse(person));
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IHttpActionResult Update(Guid id, [FromBody] UpsertPersonRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request body requerido.");
            }

            var existing = _personRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            var updated = BuildPerson(id, request);
            _personRepository.Update(updated);
            _personRepository.SaveChanges();

            return Ok(MapToResponse(updated));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IHttpActionResult Delete(Guid id)
        {
            var person = _personRepository.GetById(id);
            if (person == null)
            {
                return NotFound();
            }

            _personRepository.Delete(person);
            _personRepository.SaveChanges();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        private static Person BuildPerson(Guid id, UpsertPersonRequest request)
        {
            return new Person(
                id,
                request.FirstName,
                request.LastName,
                request.DocumentNumber,
                (PersonType)request.Type);
        }

        private static PersonResponse MapToResponse(Person person)
        {
            return new PersonResponse
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DocumentNumber = person.DocumentNumber,
                Type = (int)person.Type,
                CreatedAt = person.CreatedAt
            };
        }
    }

    public class UpsertPersonRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public int Type { get; set; }
    }

    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public int Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}