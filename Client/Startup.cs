using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                //check for authentication confirmation
                config.DefaultAuthenticateScheme = "ClientCookie";

                //Sign In Cookie
                config.DefaultSignInScheme = "ClientCookie";

                //check for allowance to do something
                config.DefaultChallengeScheme = "BinServer";

            })
                .AddCookie("ClientCookie")
                .AddOAuth("BinServer", config =>
                 {
                     config.ClientId = "client_id";
                     config.ClientSecret = "client_secret";
                     config.CallbackPath = "/oauth/callback";
                     config.AuthorizationEndpoint = "http://localhost:52362/oauth/authorize";
                     config.TokenEndpoint = "http://localhost:52362/oauth/Token";
                 });
            services.AddHttpClient();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
