﻿using System.Web.Http;
using IdentityServer3.AccessTokenValidation;
using Owin;

namespace webko
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // accept access tokens from identityserver and require a scope of 'api1'
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = "https://localhost:44333",
                ValidationMode = ValidationMode.ValidationEndpoint,

                RequiredScopes = new[] { "servers" }
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
