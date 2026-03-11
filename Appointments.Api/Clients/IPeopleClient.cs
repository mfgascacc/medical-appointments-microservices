using Appointments.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Api.Clients
{
    public interface IPeopleClient

   {
        PeoplePersonResponse GetPersonById(Guid id);
    }
}
