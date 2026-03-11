using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using People.Application.Repositories;
using People.Domain.Entities;
using People.Infrastructure.Persistence;

namespace People.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly PeopleDbContext _context;

        public PersonRepository(PeopleDbContext context)
        {
            _context = context;
        }

        public Person GetById(Guid id)
        {
            return _context.People.FirstOrDefault(x => x.Id == id);
        }

        public void Add(Person person)
        {
            _context.People.Add(person);
        }

        public void Update(Person person)
        {
            var local = _context.People.Local.FirstOrDefault(x => x.Id == person.Id);
            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Entry(person).State = EntityState.Modified;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public IEnumerable<Person> GetAll()
        {
            return _context.People.ToList();
        }

        public void Delete(Person person)
        {
            _context.People.Remove(person);
        }
    }
}
