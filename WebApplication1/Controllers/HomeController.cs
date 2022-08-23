using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
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
        [Authorize(Policy ="Claim.DOB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }
        [Authorize(Roles = "AdminTwo")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"bin@gmail.com"), 
                new Claim(ClaimTypes.DateOfBirth,"11/11/2011"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("Grandma.says","Very Nive")
            };
            var licensceClaims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Name,"Binaya Gurung"),
                new Claim("License Type","A+")
            };
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var LicensceIdentity = new ClaimsIdentity(licensceClaims, "Government");
            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, LicensceIdentity});

            HttpContext.SignInAsync(userPrincipal); 

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DoStuff()
        {
            //Doing stuff
            var builder = new AuthorizationPolicyBuilder("Schema");
            var CustomPolicy = builder.RequireClaim("Hello").Build();
            var auth_result = await _authorizationservice.AuthorizeAsync(User, CustomPolicy);
            if (auth_result.Succeeded)
            {

            }
            return View("Index");
        }
    }
}
