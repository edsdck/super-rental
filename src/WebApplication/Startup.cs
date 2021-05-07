using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebApplication.Services;

namespace WebApplication
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            
            services.AddControllersWithViews();
            
            services.AddCustomAuthentication(_configuration, _webHostEnvironment);
            
            services.AddHttpClient();
            services.AddTransient<IGatewayService, GatewayService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict });

            if (env.IsProduction())
            {
                app.UsePathBase("/mvc");
            }

            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }

    internal static class StartupExtensions
    {
        internal static IServiceCollection AddCustomAuthentication(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            var identityServerConfiguration = configuration.GetSection("IdentityServer");
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "mvccode";

                    options.Events.OnSigningOut = async e =>
                    {
                        await e.HttpContext.RevokeUserRefreshTokenAsync();
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = identityServerConfiguration["Authority"];

                    if (env.IsProduction())
                    {
                        // internal kubernetes address to a resource
                        options.MetadataAddress = $"{identityServerConfiguration["Authority"]}/.well-known/openid-configuration";
                        
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        options.Events.OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.IssuerAddress = $"{identityServerConfiguration["IdentityClientUrl"]}/connect/authorize";
                            return Task.CompletedTask;
                        };
                        
                        // Intercept the redirection so the browser navigates to the right URL in your host
                        options.Events.OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            context.ProtocolMessage.IssuerAddress = $"{identityServerConfiguration["IdentityClientUrl"]}/connect/endsession";
                            return Task.CompletedTask;
                        };
                    }

                    options.ClientId = identityServerConfiguration["ClientId"];
                    options.ClientSecret = identityServerConfiguration["Secret"];
                    options.RequireHttpsMetadata = false;
                    
                    options.ResponseType = "code";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api1");
                    options.Scope.Add("offline_access");
                    
                    // keeps id_token smaller
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    if (env.IsProduction())
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // hack: after fresh initialization of the pod for the first authentication JWKS are not found and exception is thrown.
                            // more: https://github.com/FusionAuth/fusionauth-issues/issues/368
                            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                            {
                                var client = new HttpClient();
                                var response = client
                                    .GetAsync(
                                        $"{identityServerConfiguration["Authority"]}/.well-known/openid-configuration/jwks")
                                    .Result;
                                var responseString = response.Content.ReadAsStringAsync().Result;
                                var keys = JsonConvert.DeserializeObject<JwkList>(responseString);

                                return keys.Keys;
                            },
                            ValidIssuers = new List<string>
                            {
                                identityServerConfiguration["Authority"]
                            }
                        };
                    }
                });

            // adds user and client access token management
            services.AddAccessTokenManagement(options =>
                {
                    // client config is inferred from OpenID Connect settings
                    // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
                    options.Client.Scope = "api1";
                })
                .ConfigureBackchannelHttpClient();

            services.AddUserAccessTokenClient(WebDefaults.HttpGatewayClientName, client =>
            {
                client.BaseAddress = new Uri(configuration.GetSection("ServiceUrls")["Gateway"]);
            });

            return services;
        }
    }
    
    public class JwkList
    {
        [JsonProperty("keys")]
        public List<JsonWebKey> Keys { get; set; }
    }
}