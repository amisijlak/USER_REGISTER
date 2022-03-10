using DNTCaptcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USER_REGISTER.BLL.Data;
using USER_REGISTER.BLL.Helpers;


using USER_REGISTER.BLL.Security;
using USER_REGISTER.BLL.Utils;
using USER_REGISTER.DAL;
using USER_REGISTER.DAL.Security;

namespace USER_REGISTER
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
            services.AddDbContext<USER_REGISTERDBContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, SecurityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;

                options.User.RequireUniqueEmail = true;

                // Lockout settings
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                //simplify passwords
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

                //options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<USER_REGISTERDBContext>().AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(o =>
            {
                o.Name = Constants.APPLICATION_NAME;
                o.TokenLifespan = TimeSpan.FromHours(1);
            });

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();
            services.AddTransient<IDbRepository, USER_REGISTERRepository>();
            //security
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<ISecurityService, SecurityService>();

            //Helpers
            services.AddSingleton<IUSER_REGISTERLogger, USER_REGISTERLogger>();
            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            //configure email sender
            var emailSettings = Configuration.GetSection("EmailSettings");
            services.AddSingleton(typeof(IEmailSender)
                , new EmailSender(emailSettings.GetValue<bool>("Enabled"),
                emailSettings.GetValue<string>("SmtpHost"),
                emailSettings.GetValue<int>("SmtpPort"),
                emailSettings.GetValue<bool>("UseSsl"),
                emailSettings.GetValue<string>("Username"),
                emailSettings.GetValue<string>("Password")));

            var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddDNTCaptcha(options =>
                options.UseCookieStorageProvider().ShowThousandsSeparators(false)
                .WithNoise(pixelsDensity: 25, linesCount: 3)
                .WithEncryptionKey("This is my secure key!")
                .InputNames(// This is optional. Change it if you don't like the default names.
                    new DNTCaptchaComponent
                    {
                        CaptchaHiddenInputName = "DNTCaptchaText",
                        CaptchaHiddenTokenName = "DNTCaptchaToken",
                        CaptchaInputName = "DNTCaptchaInputText"
                    })
                .Identifier("dntCaptcha")// This is optional. Change it if you don't like its default name.
            );

            services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager, RoleManager<SecurityRole> roleManager,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment envRotativa)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler(a => a.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    if (context.Request.Path.Value.ToLower().Contains("/api/"))//for api requests
                    {
                        if (exception is HttpStatusException)//set status code
                        {
                            context.Response.StatusCode = (int)((HttpStatusException)exception).StatusCode.GetIntValue();
                        }

                        await context.Response.WriteAsync(exception.ExtractInnerExceptionMessage());
                    }
                    else//for general requests
                    {
                        context.Response.Redirect("/Error");
                    }
                }));
            }

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            RotativaConfiguration.Setup(envRotativa);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            var services = app.ApplicationServices.GetService<IServiceScopeFactory>();
            var context = services.CreateScope().ServiceProvider.GetRequiredService<USER_REGISTERDBContext>();
            context.Database.Migrate();

            IdentityDataInitializer.SeedData(userManager, roleManager);
        }

        //public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        //{
        //    RotativaConfiguration.Setup(env);
        //}
    }
}
