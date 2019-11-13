using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIBearerTokenExample.Models
{
    public class ApiLog
    {
        public string Host { get; set; }
        public string Headers { get; set; }
        public string StatusCode { get; set; }
        public string RequestBody { get; set; }
        public string RequestedMethod { get; set; }
        public string UserHostAddress { get; set; }
        public string Useragent { get; set; }
        public string AbsoluteUri { get; set; }
        public string RequestType { get; set; }
        public string Guid { get; set; }
    }
}