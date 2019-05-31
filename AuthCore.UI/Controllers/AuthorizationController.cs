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
    [Authorize]
    public class AuthorizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
                 
        
        /// <summary>
        /// This view authenticates and adds a Role to the user to be able to access actions with Authorization protection
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginWithRole(string name, string role)
        {
            //validate name not empty and role is Basic or Admin
            if (string.IsNullOrEmpty(name) && (role != "Basic" || role != "Admin"))
            {
                RedirectToAction(nameof(Index));
            }

            //Add role to User with no duplicates
            if (!User.IsInRole(role))
            {
                //Add role claim to the existing user
                var claims = new List<Claim> { new Claim(ClaimTypes.Role, role) };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var user = HttpContext.User;
                user.AddIdentity(identity);
                var principal = user;

                //SignIn
                await AuthenticationHttpContextExtensions.SignInAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }

            var redirect = string.Empty;

            if (role == "Admin")
            {
                redirect = "AuthenticateAndAuthorizeWithRole";
            }
            else
            {
                redirect = "AuthenticateAndAuthorize";
            }

            return RedirectToAction(redirect);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext,  CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(Index), "Home");
        }


        /// <summary>
        /// Validates that User is authenticated and authorized as per policy "UserIsAdmin"
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "UserIsAdmin")]
        public IActionResult ManageUser()
        {
            return View();
        }

        public IActionResult AuthenticateAndAuthorize()
        {
            return View();
        }

        public IActionResult AuthenticateAndAuthorizeWithRole() => View();

    }
}