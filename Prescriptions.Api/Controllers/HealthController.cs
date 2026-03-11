using System.Web.Http;

namespace Prescriptions.Api.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Prescriptions API running");
        }
    }
}