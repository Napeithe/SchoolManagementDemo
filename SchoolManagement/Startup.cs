using System;
using System.Globalization;
using System.Reflection;
using EmailSender;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model;
using Model.Auth;
using Model.Domain;
using SchoolManagement.Infr;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Identity;
using StackExchange.Profiling.Storage;

namespace SchoolManagement
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

            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<SchoolManagementContext>(options =>
                {
                    options.EnableSensitiveDataLogging();
                    options.UseNpgsql(connectionString, x => x.MigrationsAssembly(nameof(SchoolManagement)))
                        .ConfigureWarnings(warnings => warnings.Log(RelationalEventId.QueryClientEvaluationWarning));
                });


            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<SchoolManagementContext>()
                .AddErrorDescriber<PolishIdentityErrorDescriber>()
                .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory<User, Role>>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(90);

                options.LoginPath = "/Account/Index";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            services.AddAuthorization(options =>
            {
                foreach (var key in Policy.GetAllPermissions())
                {
                    options.AddPolicy(key,
                        policy => { policy.RequireClaim(CustomClaimTypes.Permission, key); });
                }
            });

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelper, UrlHelperProxy>();

            services.AddEmailService(Configuration);

            services.AddSession(opts => { opts.Cookie.IsEssential = true; });

            ValidatorOptions.LanguageManager = new CustomLanguageManager { Culture = new CultureInfo("pl") };


            services.AddMiniProfiler(options =>
            {
                // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control storage
                // (default is 30 minutes in MemoryCacheStorage)
                (options.Storage as MemoryCacheStorage).CacheDuration = TimeSpan.FromMinutes(60);

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();


                // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
                // (defaults to true, and connection opening/closing is tracked)
                options.TrackConnectionOpenClose = true;
            }).AddEntityFramework();


            services.AddMvc(o => o.WithFeatureConvention())
                .AddFluentValidation(fv=>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .AddRazorOptions(o => { o.WithFeatureFolderLocalizations(); })
                .AddSessionStateTempDataProvider()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiniProfiler();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Calendar}/{action=Index}");
            });
        }
    }
}
