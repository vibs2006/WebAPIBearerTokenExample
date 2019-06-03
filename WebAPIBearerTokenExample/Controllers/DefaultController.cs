using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace WebAPIBearerTokenExample.Controllers
{
    [RoutePrefix("v1")]
    public class DefaultController : ApiController
    {

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
