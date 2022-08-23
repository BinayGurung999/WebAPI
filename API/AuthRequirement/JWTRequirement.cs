using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.AuthRequirement
{
    public class JWTRequirement : IAuthorizationRequirement
    {

    } 
    public class JWTRequirementHandler : AuthorizationHandler<JWTRequirement>
    {
        private readonly HttpClient _httpclient;
        private readonly HttpContext _httpcontext;
        public JWTRequirementHandler(IHttpClientFactory httpclientfactory,IHttpContextAccessor httpcontextaccessor)
        {
            _httpclient = httpclientfactory.CreateClient();
            _httpcontext = httpcontextaccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JWTRequirement requirement)
        {
            if(_httpcontext.Request.Headers.TryGetValue("Authorization",out var authheader))
            {
                var accesstoken = authheader.ToString().Split(' ')[1];
                var Response = await _httpclient.GetAsync($"http://localhost:52362/oauth/validate?access_token={accesstoken}");
                if(Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        } 
    }
}
