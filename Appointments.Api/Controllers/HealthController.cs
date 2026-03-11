using System.Web.Http;

namespace Appointments.Api.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Appointments API running");
        }
    }
}