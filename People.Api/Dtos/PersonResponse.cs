using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace People.Api.Dtos
{
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