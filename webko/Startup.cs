using System.Web.Http;
using IdentityServer3.AccessTokenValidation;
using Owin;

namespace webko
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // accept access tokens from identityserver and require a scope of 'desktop api'
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://localhost:44333",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] { "desktopApi", "servicesApi" },
            });


            // configure web api
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // require authentication for all controllers
            config.Filters.Add(new AuthorizeAttribute());

            app.UseWebApi(config);
        }
    }
}
