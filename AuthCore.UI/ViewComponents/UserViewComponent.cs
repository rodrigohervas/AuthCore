using AuthCore.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace AuthCore.UI.ViewComponents
{
    public class UserViewComponent: ViewComponent
    {

        public UserViewComponent()
        {

        }

        public IViewComponentResult Invoke(int id)
        {
            var user = GetUser(id);

            return View("Default", user);
        }

        private UserViewModel GetUser(int id)
        {
            var user = new UserViewModel
            {
                Name = User.Identity.Name,
                Email = UserClaimsPrincipal.FindFirst(c => c.Type == ClaimTypes.Email).Value
            };

            return user;
        }
    }
}
