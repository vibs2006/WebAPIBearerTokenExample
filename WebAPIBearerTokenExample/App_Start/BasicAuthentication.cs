using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebAPIBearerTokenExample.App_Start
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                
                if (actionContext.Request.Headers.Authorization != null)
                {
                    if (actionContext.Request.Headers.Authorization.Scheme.ToLower() != "basic")
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        var obj = new
                        {
                            Status = "Failure",
                            Description = "Only Basic Authentication is supported"
                        };
                        response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                        response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        actionContext.Response = response;
                    }
                    else
                    {
                        //actionContext.Request.Headers.Authorization.Scheme
                        //Commented facility to excluded brand
                        var authToken = actionContext.Request.Headers.Authorization.Parameter;
                        //IEnumerable<string> values;
                        //if (actionContext.Request.Headers.TryGetValues("brand", out values))
                        //{
                        //    _brand = values.First();
                        //}

                        if (true)
                        {
                            // decoding authToken we get decode value in 'Username:Password' format  
                            var decodeauthToken = System.Text.Encoding.UTF8.GetString(
                                Convert.FromBase64String(authToken));

                            // spliting decodeauthToken using ':'   
                            var arrUserNameandPassword = decodeauthToken.Split(':');

                            long Customer_ID = 0;
                            // at 0th postion of array we get username and at 1st we get password  
                            if (IsAuthorizedUser(arrUserNameandPassword[0], arrUserNameandPassword[1]))
                            {
                                ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(arrUserNameandPassword[0]), null);
                                identity.AddClaim(new Claim("Guid", Guid.NewGuid().ToString()));
                                identity.AddClaim(new Claim("Customer_ID", Customer_ID.ToString()));
                                identity.AddClaim(new Claim("Username", identity.Name));
                                identity.AddClaim(new Claim(ClaimTypes.Email, arrUserNameandPassword[0]));
                                Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
                            }
                            else
                            {
                                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                                var obj = new
                                {
                                    Status = "Failure",
                                    Description = "Invalid username and/or password"
                                };
                                response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                                actionContext.Response = response;
                            }
                        }
                        //else
                        //{
                        //    actionContext.Response = actionContext.Request
                        //       .CreateResponse(HttpStatusCode.Unauthorized);
                        //}
                    }
                }
                else
                {
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    var obj = new
                    {
                        Status = "Failure",
                        Description = "No Authentication Method Specified"
                    };
                    response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    actionContext.Response = response;
                }
            }
            catch (Exception Ex)
            {

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                var obj = new
                {
                    Status = "Failure",
                    Description = "Invalid HTTP Request resulting in server error"
                };
                response.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                actionContext.Response = response;                
            }
        }

        private bool IsAuthorizedUser(string username, string password)
        {
            if (ConfigurationManager.AppSettings.Get("Username") == username && ConfigurationManager.AppSettings.Get("Password") == password)
            {
                return true;
            }

            return false;
        }

        
    }
}