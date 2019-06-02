using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

    }
}
