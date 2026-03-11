using People.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Application.Repositories
{
    public interface IPersonRepository
    {
        Person GetById(Guid id);
        void Add(Person person);
        void Update(Person person);
        int SaveChanges();


        IEnumerable<Person> GetAll();
        void Delete(Person person);
    }


}
