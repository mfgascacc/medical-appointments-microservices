using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace People.Api.Security
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly JwtTokenService _tokenService = new JwtTokenService();

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var header = actionContext.Request.Headers.Authorization;
            if (header == null ||
                !"Bearer".Equals(header.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return _tokenService.ValidateToken(header.Parameter);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(
                HttpStatusCode.Unauthorized,
                new { message = "Unauthorized. Provide a valid Bearer token." });
        }
    }
}
