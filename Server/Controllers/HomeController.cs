using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private IAuthorizationService _authorizationservice;
        public HomeController(IAuthorizationService authorizationservice)
        {
            _authorizationservice = authorizationservice;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"Some_Id"),
                new Claim("granny","cookie")
            };
            var secretbytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretbytes);
            var algorithm = SecurityAlgorithms.HmacSha256;
            var signcredentials = new SigningCredentials(key,algorithm);
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiance,
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signcredentials
                );
            var TokenJson = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { access_token = TokenJson });
        }
    }
}
