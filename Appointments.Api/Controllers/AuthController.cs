using Appointments.Api.Security;
using System;
using System.Web.Http;

namespace Appointments.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private readonly JwtTokenService _tokenService = new JwtTokenService();

        [HttpPost]
        [Route("token")]
        public IHttpActionResult Token([FromBody] TokenRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            if (!_tokenService.ValidateCredentials(request.Username, request.Password))
            {
                return Unauthorized();
            }

            const int expiresInMinutes = 120;
            var token = _tokenService.GenerateToken(request.Username, expiresInMinutes);

            return Ok(new TokenResponse
            {
                AccessToken = token,
                TokenType = "Bearer",
                ExpiresIn = (int)TimeSpan.FromMinutes(expiresInMinutes).TotalSeconds
            });
        }
    }

    public class TokenRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
