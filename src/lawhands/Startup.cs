using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using lawhands.Data;
using lawhands.Models;
using lawhands.Services;
using Microsoft.AspNetCore.Mvc;

namespace lawhands
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            // Temporary commented out, to debug Development mode in Azure    
            //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            CurrentEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string webRootPath = CurrentEnvironment.ContentRootPath;
            var appDataDirectory = Path.Combine(webRootPath, "data");
            if (!Directory.Exists(appDataDirectory))
            {
                Directory.CreateDirectory(appDataDirectory);
            }
            var sqliteDbFilePath = Path.Combine(appDataDirectory, "identity.sqlite");
            Console.WriteLine(sqliteDbFilePath);
            // var connection = Configuration["Production:SqliteConnectionString"];
            var connection = "Data Source=" + sqliteDbFilePath;
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connection)
            );

            // Add framework services.
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Cookies.ApplicationCookie.AccessDeniedPath =
                    new Microsoft.AspNetCore.Http.PathString("/Account/Forbidden");
                options.Cookies.ApplicationCookie.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                options.Cookies.ApplicationCookie.AutomaticChallenge = true;
                options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.AdministratorsOnly, policy => policy.RequireRole(RoleNames.Administrator));
            });
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddMvc().AddViewLocalization();
            services.AddLocalization();
           
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.Initialize(app.ApplicationServices);
        }
    }
}
