using Basics.AuthorizationRequirements;
using Basics.Controllers;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "gg.cookie";
                    config.LoginPath = "/Home/Authenticate";
                }); 
            services.AddAuthorization(config =>
            {
                //global authorization filter
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                    .RequireAuthenticatedUser()
                    .Build();

                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role,"Admin"));
                config.AddPolicy("Claim.DOB", policyBuilder =>
                 {
                     policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                 });
            });
             
            services.AddScoped<IAuthorizationHandler,CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config =>
            {
                config.Filters.Add(new AuthorizeFilter());
            });
            services.AddRazorPages()
                .AddRazorPagesOptions(config =>
                {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    config.Conventions.AuthorizePage("/Razor/Policy","Admin");

                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
