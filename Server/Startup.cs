using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config =>
                 {
                     var secretbytes = Encoding.UTF8.GetBytes(Constants.Secret);
                     var key = new SymmetricSecurityKey(secretbytes);
                     config.Events = new JwtBearerEvents()
                     {
                         OnMessageReceived = context =>
                         {
                             if (context.Request.Query.ContainsKey("access_token"))
                             {
                                 context.Token = context.Request.Query["access_token"];
                             }
                             return Task.CompletedTask;
                         }
                     };
                     config.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidIssuer = Constants.Issuer,
                         ValidAudience = Constants.Audiance,
                         IssuerSigningKey = key
                     };
                 });
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
