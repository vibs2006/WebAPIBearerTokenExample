using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using WebAPIBearerTokenExample.Common;
using WebAPIBearerTokenExample.Common.ControllerMethods;

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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var lastModifiedTime = new System.IO.FileInfo(assembly.Location).LastWriteTime;
            //var time = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
            var response = new HttpResponseMessage
            {
                Content = new StringContent($"<html><body>Web API 2 Hosting Successful using OWIN Framework. Last Modified on ({lastModifiedTime.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture)}) </body></html>")
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Default/Ping")]
        public IHttpActionResult Ping()
        {
            Random rd = new Random();
            var number = rd.Next(1000, 5000);
            Thread.Sleep(number);
            var val = CustomLogger.GetLoggerInstance(Request).GetReferenceId;
            return Ok(new {
                Status = "Success",
                Descrition = "Anonymous Access tested Succesfully",
                val = val,
                TotalInstancesCount = CustomLogger.GetTotalInstanceCount,
                ProcessedTime = number
            });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Default/GetTotalInstances")]
        public IHttpActionResult GetTotalInstances()
        {          
            var val = CustomLogger.GetLoggerInstance(Request).GetReferenceId;
            return Ok(new
            {
                Status = "Success",
                Descrition = "Anonymous Access tested Succesfully",
                val = val,
                TotalInstancesCount = CustomLogger.GetTotalInstanceCount                
            });
        }

        [Authorize]
        [HttpGet]
        [Route("Default/TestAuthorization")]
        public IHttpActionResult TestAuthorization()
        {
            var logger = CustomLogger.GetLoggerInstance(Request);
            logger.AppendLog("This is midway appending logger line");
            Thread.Sleep(2500);
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
