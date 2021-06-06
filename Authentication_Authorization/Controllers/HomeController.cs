using Authentication_Authorization.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication_Authorization.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet("denied")]
        public IActionResult Denied()
        {
            return View();
        }


        // variant 1 (if we don't need tokens)
        [Authorize(Roles ="Admin")]
        public IActionResult Secured()
        {
            return View();
        }

        // variant 2 (if we need tokens)

        //[Authorize(Roles ="Admin")]
        //public async Task<IActionResult>  Secured()
        //{
        //    var idToken = await HttpContext.GetTokenAsync("id_token"); 

        //    return View();
        //}





        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl; // we pass current url to view

            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Validate(string username, string password, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (username=="bob" && password == "pizza")
            {

                var claims = new List<Claim>();
                claims.Add(new Claim("username", username));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, username));
                claims.Add(new Claim(ClaimTypes.Name, "Ramin Agayev")); // we add this temporary. It have to come form DB.
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                return Redirect(returnUrl);
            }

            TempData["Error"] = "Error. Username or Password is invalid.";
            return View("login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            //return Redirect("/");

            return Redirect("https://www.google.com/accounts/Logout?continue=https://appengine.google.com/_ah/logout?continue=https://localhost:44356"); // for security reason
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
