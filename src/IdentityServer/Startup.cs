using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.Data;
using IdentityServer.Data.Entities;
using IdentityServer.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class Startup
    {
        private static string DatabaseConnectionStringName => "IdentityServerDatabase";
        
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            // setup for Identity
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString(DatabaseConnectionStringName)));
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //TODO: use these requirements only in dev env
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 2;
            });
            
            // setup for IdentityServer
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var identityBuilder = services
                .AddIdentityServer()
                .AddDeveloperSigningCredential(); //TODO: use only in dev env
            
            identityBuilder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        _configuration.GetConnectionString(DatabaseConnectionStringName),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        _configuration.GetConnectionString(DatabaseConnectionStringName),
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAspNetIdentity<ApplicationUser>()
                .Services.AddTransient<IProfileService, ProfileService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            
            app.UseRouting();
            
            app.UseIdentityServer();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();

            if (serviceScope is null)
            {
                throw new NullReferenceException("IServiceScope is null. Cannot proceed with database migration.");
            }
            
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
            
            // Identity migration & seed
            var identityContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            identityContext.Database.Migrate();

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            if (!identityContext.Users.Any())
            {
                var bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };

                _ = userManager.CreateAsync(bob, "bob").Result;
            }
        }
    }
}   