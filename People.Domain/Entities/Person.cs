using People.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace People.Domain.Entities
{
    public class Person
    {
        protected Person()
        {
            // Required by EF6 for materialization.
        }

        public Guid Id { get; private set; }
        public string FirstName {  get; private set; }
        public string LastName {  get; private set; }
        public string DocumentNumber { get; private set; }
        public PersonType Type { get; private set; }    
        public DateTime CreatedAt {  get; private set; }

        public Person(
            Guid id, 
            string firstName,
            string lastName,
            string documentNumber,
            PersonType type)
        {
            if (id == Guid.Empty) throw new ArgumentNullException("Id invalido");
            if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("Primer nombre requerido");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("LastName requerido.");
            if (string.IsNullOrWhiteSpace(documentNumber)) throw new ArgumentException("DocumentNumber requerido.");
            Id = id;
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            DocumentNumber = documentNumber.Trim();
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
