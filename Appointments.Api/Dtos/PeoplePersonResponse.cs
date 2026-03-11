using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Appointments.Api.Dtos
{
    public class PeoplePersonResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public int Type { get; set; } // 1 Doctor, 2 Patient
        public DateTime CreatedAt { get; set; }
    }
}