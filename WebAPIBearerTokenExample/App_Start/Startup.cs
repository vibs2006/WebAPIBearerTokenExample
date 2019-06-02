using System;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebAPIBearerTokenExample;

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
                            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                            context.Validated(oAuthIdentity);
                        }
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