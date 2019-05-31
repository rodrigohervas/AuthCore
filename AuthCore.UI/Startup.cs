using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthCore.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Add MVC
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Add Authentication
            //Choose the authentication scheme as Cookies
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //Add options inside AddCookie method to customize behaviour when user is not authorized
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/ErrorNotLogedIn"; //redirection if user is not loged-in
                    options.AccessDeniedPath = "/Home/ErrorAccessDenied"; //redirection if the authorization fails
                    options.LogoutPath = "/Home/Index";
                    options.Cookie.Name = "AuthCoreAppCookie";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(45);
                });

            //Add Authorization and configure a policies for "UserIsAdmin" with a Role of "Admin", and
            // "UserIsBasic" with a Role of "Basic". Both policies require also for the use to be authenticated.
            services.AddAuthorization(options => 
            {
                var AdminPolicy = Configuration.GetSection("Policies").GetSection("UserIsAdmin");
                var BasicPolicy = Configuration.GetSection("Policies").GetSection("UserIsBasic");
                options.AddPolicy(AdminPolicy.Key, p => p.RequireAuthenticatedUser().RequireRole(AdminPolicy.Value));
                options.AddPolicy(BasicPolicy.Key, p => p.RequireAuthenticatedUser().RequireRole(BasicPolicy.Value));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //Add the Authentication midleware to the pipeline
            app.UseAuthentication();

            //Configure MVC routes
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
