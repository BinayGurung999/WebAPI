using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Server;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    public class OAuthController : Controller
    {
        [HttpGet]
        public IActionResult Authorize(string response_type,string client_id,string redirect_uri,string scope,string state)
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri);
            query.Add("state", state);
            return View(model:query.ToString());
        }
        [HttpPost]
        public IActionResult Authorize(string username,string redirect_uri,string state)
        {

            return RedirectToAction("");
        }   
        public async Task<IActionResult> TokenAsync(string grant_type, string code, string redirect_uri, string client_id)
        {
            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub,"Some_Id"),
                new Claim("granny","cookie")
            };
            var secretbytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretbytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signcredentials = new SigningCredentials(key, algorithm);
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signcredentials
                );
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            var responseObject = new
            {
                access_token,
                token_type="Bearer",
                raw_claim = "oauth",
            };
            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes,0,responseBytes.Length);
            return Redirect(redirect_uri);
        }
        [Authorize]
        public IActionResult Validate()
        {
            if (HttpContext.Request.Query.TryGetValue("access_token",out var accessToken))
            {

                return Ok();
            }
            return BadRequest();
        }
    }
}
