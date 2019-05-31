using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCore.UI.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Sign the user In using cookies
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string name, string password, string email)
        {
            //validate that there is user data
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                RedirectToAction(nameof(Index), nameof(HomeController));
            }

            //login the user: ClaimsIdentity, ClaimsPrincipal and SignInAsync()
            //Create identity clames for the user data
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //create a claims principal for the user
            var principal = new ClaimsPrincipal(identity);

            //login the user
            await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("LogedInUsers","Authentication");
        }

        /// <summary>
        /// Sign the user Out
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult LogedInUsers()
        {
            return View();
        }
    }
}