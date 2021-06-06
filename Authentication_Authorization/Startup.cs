using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication_Authorization
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
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            { // adding for google ////////////////
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GoogleOpenID";// variant 1   (GoogleDefaults.AuthenticationScheme;) variant 2
                ///////////////////////////////////////////////////////////////////////////////////////
            })
                .AddCookie(options => // added manualy for cookies
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/denied"; // manually. If user do not pass authentication he redirect to denied page 
                    //options.Events = new CookieAuthenticationEvents()
                    //{
                    //    OnSigningIn = async context =>
                    //    {
                    //        // manually
                    //        // here we check name user and if it's corresponds to his name we add role Admin to him
                    //        var principal = context.Principal;
                    //        if (principal.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                    //        {
                    //            if (principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "bob")
                    //            {
                    //                var claimsIdentity = principal.Identity as ClaimsIdentity;
                    //                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    //            }
                    //        }

                    //        await Task.CompletedTask;
                    //    },
                    //    OnSignedIn = async context =>
                    //      {
                    //          await Task.CompletedTask;
                    //      },
                    //    OnValidatePrincipal = async context =>
                    //    {
                    //        await Task.CompletedTask;
                    //    }
                    //};
                })
                .AddOpenIdConnect("GoogleOpenID", options => // connect with google (variant 2) (if we want get more info from google about user, like picture to show on site) 
                {
                    options.Authority = "https://accounts.google.com";
                    options.ClientId = "937054166175-t2jjosmno74poo8nmm14ktkbinl6n1ns.apps.googleusercontent.com";
                    options.ClientSecret = "nOM7CwXzeQqCyJlPT7CakqWj";
                    options.CallbackPath = "/Authentication_Authorization";
                    //options.AuthorizationEndpoint = "?prompt=consern"; // if you can enter to google account directly. It's give right for choose
                    //options.SaveTokens = true; // if we need tokens
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnTokenValidated = async context =>
                        {
                            if (context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "106807182706056831861")
                            {
                                var claim = new Claim(ClaimTypes.Role, "Admin");
                                var claimIdentity = context.Principal.Identity as ClaimsIdentity;
                                claimIdentity.AddClaim(claim);  
                            }
                        }
                    };
                });
                //.AddGoogle(options => // connect with google (variant 1) // manualy
                //{
                //    options.ClientId = "937054166175-t2jjosmno74poo8nmm14ktkbinl6n1ns.apps.googleusercontent.com";
                //    options.ClientSecret = "nOM7CwXzeQqCyJlPT7CakqWj";
                //    options.CallbackPath = "/Authentication_Authorization";
                //   // options.AuthorizationEndpoint = "?prompt=consern"; // if you cen enter to google account directly. It's give right for choose
                //}); // manually
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // manualy
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
