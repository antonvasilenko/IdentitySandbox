using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using Microsoft.Owin.Hosting;
using Owin;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace IdentitySrv
{
    class Program
    {
        static void Main(string[] args)
        {
            // logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                })
                .WriteTo.LiterateConsole(outputTemplate: "{Timestamp:HH:MM} [{Level}] ({Name:l}){NewLine} {Message}{NewLine}{Exception}").CreateLogger();

            // hosting identityserver
            using (WebApp.Start<Startup>("https://localhost:44333"))
            {
                Console.WriteLine("server running...");
                Console.ReadLine();
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var options = new IdentityServerOptions
            {
                Factory = new IdentityServerServiceFactory()
                            .UseInMemoryClients(Clients.Get())
                            .UseInMemoryScopes(Scopes.Get())
                            .UseInMemoryUsers(new List<InMemoryUser>())
            };

            app.UseIdentityServer(options);
        }
    }

    static class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>
        {
            new Scope{Name = "desktop"},
            new Scope{Name = "mobile"},
            new Scope{Name = "servers"},
        };
        }
    }

    static class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
        {
           // no human involved
            new Client
            {
                ClientName = "Silicon-only Client",
                ClientId = "silicon",
                Enabled = true,
                AccessTokenType = AccessTokenType.Reference,

                Flow = Flows.ClientCredentials,

                ClientSecrets = new List<Secret>
                {
                    new Secret("F621F470-9731-4A25-80EF-67A6F7C5F4B8".Sha256())
                },

                AllowedScopes = new List<string>
                {
                    "servers"
                }
            }
        };
        }
    }
}
