using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Http;

namespace WebAPIBearerTokenExample.Controllers
{
    [RoutePrefix("v1")]
    public class DefaultController : ApiController
    {

        [HttpGet]
        [AllowAnonymous]
        [Route("Default/Index")]
        public HttpResponseMessage Index()
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent("<html><body>Web API 2 Hosting Successful using OWIN Framework</body></html>");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Default/Ping")]
        public IHttpActionResult Ping()
        {
            return Ok(new {
                Status = "Success",
                Descrition = "Anonymous Access tested Succesfully"
            });
        }

        [Authorize]
        [HttpGet]
        [Route("Default/TestAuthorization")]
        public IHttpActionResult TestAuthorization()
        {

            return Ok(new {
                Status = "Success",
                Description = "Authorization Test Successful"
            });
        }

        [Authorize]
        [HttpGet]
        [Route("Default/TestClaims")]
        public IHttpActionResult TestClaims()
        {
            ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;

            var claims = claimsIdentity.Claims.Select(x => new { type = x.Type, value = x.Value });

            return Ok(claims);
        }

    }
}
