using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("Membership");
            services.AddDbContextPool<ApplicationIdentityDbContext>(options =>
                    options.UseMySql(
                        connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<ApplicationIdentityUser, IdentityRole>()
                .AddUserManager<UserManager<ApplicationIdentityUser>>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Home/Index");
                options.ReturnUrlParameter = new PathString("/Admin/AdminIndex");
                //other properties
            });

            services.AddMvc().AddRazorRuntimeCompilation();
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<EmailSenderOptions>(options =>
            {
                options.ApiKey = _configuration["ExternalProviders:SendGrid:ApiKey"];
                options.SenderEmail = _configuration["ExternalProviders:SendGrid:SenderEmail"]; 
                options.SenderName = _configuration["ExternalProviders:SendGrid:SenderName"];
            });
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
            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseRouting();
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
