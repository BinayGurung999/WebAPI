using IdentityExample.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _usermanager;
        private readonly SignInManager<IdentityUser> _signinmanager;
        private readonly IEmailService _emailservice;
        public HomeController(UserManager<IdentityUser> usermanager,SignInManager<IdentityUser> signinmanager,IEmailService emailservice)
        {
            _usermanager = usermanager;
            _signinmanager = signinmanager;
            _emailservice = emailservice;
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {
            //login functions
            var user =await _usermanager.FindByNameAsync(username);
            if(user != null)
            {
                //sign in
                var signresult=await _signinmanager.PasswordSignInAsync(user, password,false,false);
                if (signresult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string username,string password)
        {
            //register functions
            var user = new IdentityUser
            {
                UserName = username,
                Email = "",
            };
            var result= await _usermanager.CreateAsync(user,password);
            if (result.Succeeded)
            {
                var code = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
                var link = Url.Action(nameof(VerifyEmail),"Home",new { UserId=user.Id,code},Request.Scheme,Request.Host.ToString());
                await _emailservice.SendAsync("test@test.com","email verify", $"<a href =\"{link}\">Verify Email</a>",true);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> VerifyEmail(string UserId,string code)
        {
            var user = await _usermanager.FindByIdAsync(UserId);
            if (user == null) return BadRequest();
            var result = await _usermanager.ConfirmEmailAsync(user,code);
            if (result.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        public async Task<IActionResult> Logout()
        {
            await _signinmanager.SignOutAsync();
            return RedirectToAction("Index");
        }

        public IActionResult EmailVerification() => View();
    }
}
