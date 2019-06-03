using System;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebAPIBearerTokenExample;
using System.IO;
using System.Text;
using System.Diagnostics;

[assembly: OwinStartup(typeof(Startup))]

namespace WebAPIBearerTokenExample
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            OAuthOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthAuthorizationServerProvider()
                {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                    OnValidateClientAuthentication = async (context) =>
                    {
                        context.Validated();
                    },
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                    OnGrantResourceOwnerCredentials = async (context) =>
                    {
                        if (context.UserName == "test.test@mail.com" && context.Password == "test123")
                        {

                           // var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                            //identity.AddClaim(new Claim("sub", context.UserName));
                            //identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

                            //context.Validated(identity);

                            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                            oAuthIdentity.AddClaim(new Claim("sub", context.UserName));
                            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));
                            context.Validated(oAuthIdentity);
                        }
                        else if (context.UserName == "test.test2@mail.com" && context.Password == "test123")
                        {
                            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                            oAuthIdentity.AddClaim(new Claim("sub", context.UserName));
                            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Email, context.UserName));
                            context.Validated(oAuthIdentity);
                        }
                        else
                        {
                            context.SetError("Invalid Username / Password", "Invalid Username / Password");
                            context.Rejected();
                            
                        }


                        //Stream s = null;
                        //context.Response.Body.CopyTo(s);
                        //string body = new StreamReader(s).ReadToEnd();
                        //byte[] data = Convert.FromBase64String(body);
                        //string decodedString = Encoding.UTF8.GetString(data);
//                        using(var bufferStream = new MemoryStream())
//                        {
//                            var orgBodyStream = context.Response.Body;
//                            context.Response.Body = bufferStream;

//                            bufferStream.Seek(0, SeekOrigin.Begin);
//#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//                            bufferStream.CopyToAsync(orgBodyStream);
//#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
//                            context.Response.Body = orgBodyStream;
//                            string s1 = new StreamReader(orgBodyStream).ReadToEnd();
//                            Trace.WriteLine(s1);
//                        }
                    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                },
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1)
            };

            app.UseOAuthBearerTokens(OAuthOptions);
        }
    }
}