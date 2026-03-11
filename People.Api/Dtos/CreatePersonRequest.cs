using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace People.Api.Dtos
{
    public class CreatePersonRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public int Type { get; set; } // 1 Doctor, 2 Patient
    }
}