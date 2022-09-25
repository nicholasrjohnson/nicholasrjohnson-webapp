using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authentication.Facebook;
//using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;
using MySqlConnector;
using webapp.Data;
using webapp.Services;

namespace webapp 
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Application builder for middleware.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseHttpException(this IApplicationBuilder application)
        {
            return application.UseMiddleware<HttpExceptionMiddleware>();
        }
    } 

    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            StaticConfig = configuration;
        }

         /// <summary>
        /// Gets the static config.
        /// </summary>
        public static IConfiguration StaticConfig { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("Membership");
            services.AddDbContextPool<ApplicationIdentityDbContext>(options =>
                    options.UseMySql(
                        connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<ApplicationIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 15;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
                .AddUserManager<UserManager<ApplicationIdentityUser>>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();
            
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2)
            );
           services.AddAuthentication()
            .AddCookie("www.nicholasrjohnson.scheme", options =>
            {
                options.Cookie.Name = "www.nicholasrjohnson.cookie";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Errors/AccessDenied";
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });
            
            services.AddAuthorization(); 
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1);
            });

            services.AddResponseCaching();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Index");
                options.ReturnUrlParameter = new PathString("/Admin/AdminIndex");
            });

            var mvc = services.AddMvc();
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<EmailSenderOptions>(options =>
            {
                options.ApiKey = _configuration["ExternalProviders:SendGrid:ApiKey"];
                options.SenderEmail = _configuration["ExternalProviders:SendGrid:SenderEmail"]; 
                options.SenderName = _configuration["ExternalProviders:SendGrid:SenderName"];
            });

#if DEBUG
            mvc.AddRazorRuntimeCompilation();
#endif
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
                app.UseExceptionHandler("/Views/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }
            //app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    /// <summary>
    /// The HttpException.
    /// </summary>
    public class HttpException : Exception
    {
        public readonly int httpStatusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">Http status code.</param>
        public HttpException(int httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The status code.</param>
        public HttpException(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpException"/> class.
        /// </summary>
        /// <param name="httpStatusCode">The status code.</param>
        /// <param name="message">The message for the http code message.</param>
        public HttpException(int httpStatusCode, string message)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }
    }

    /// <summary>
    /// The Http middleware to work with the permsissions.
    /// </summary>
    internal class HttpExceptionMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next step in the pipeline.</param>
        public HttpExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                context.Response.StatusCode = httpException.httpStatusCode;
                var responseFeature = context.Features.Get<IHttpResponseFeature>();
                responseFeature.ReasonPhrase = httpException.Message;
            }
        }
    }
}
