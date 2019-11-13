using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace WebAPIBearerTokenExample.Common.ControllerMethods
{
    public static class Methods
    {
        public static string GetCurrentRequestGuid(HttpRequestMessage request)
        {
            var guid = request.Headers.GetValues("GuidValue").FirstOrDefault();
            return guid == null ? string.Empty : guid;
        }
    }
}