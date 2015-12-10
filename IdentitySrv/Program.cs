using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                            .UseInMemoryUsers(Users.Get())
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
            new Scope{Name = "desktopApi", Claims = new List<ScopeClaim>(){new ScopeClaim("orgUnit")}},
            new Scope{Name = "mobile api"},
            new Scope{Name = "servicesApi"},
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
                    ClientName = "Alarm Builder",
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
                        "servicesApi"
                    }
                },

                // human is involved
                new Client
                {
                    ClientName = "Desktop App",
                    ClientId = "carbon",
                    Enabled = true,
                    AccessTokenType = AccessTokenType.Reference,

                    Flow = Flows.ResourceOwner,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("21B5F798-BE55-42BC-8AA8-0025B903DC3B".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        "desktopApi"
                    }
                }
            };
        }
    }

    static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
        {
            new InMemoryUser
            {
                Username = "bob",
                Password = "secret",
                Subject = "1", // org unit id ???
                Claims = new []
                {
                    new Claim("orgUnit", "headorg"), 
                }
            },
            new InMemoryUser
            {
                Username = "alice",
                Password = "secret",
                Subject = "2",
                Claims = new []
                {
                    new Claim("orgUnit", "antonv"), 
                }
            }
        };
        }
    }
}
