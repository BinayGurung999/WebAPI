using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
namespace Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpclient;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpclient = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public async Task<IActionResult> SecretAsync()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            _httpclient.DefaultRequestHeaders.Add("Authorization",$"Bearer{token}");
            var ServerResponse = await _httpclient.GetAsync("http://localhost:52362/");
            var APIResponse = await _httpclient.GetAsync("http://localhost:59999");
            return View();
        }
    }
}
