
    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Data.Entity;
using eShopLegacyMVC.Models;
using eShopLegacyMVC.Models.Infrastructure;
using eShopLegacyMVC.Modules;

    namespace eShopLegacyMVC
    {
        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);

                // Store configuration in static ConfigurationManager
                ConfigurationManager.Configuration = builder.Configuration;

                // Add DbContext for Entity Framework 6
                builder.Services.AddScoped<CatalogDBContext>(provider =>
                {
                    // Create and return a new instance of CatalogDBContext
                    // Entity Framework 6 will use the connection string from its configuration
                    return new CatalogDBContext();
                });

                // Add services to the container (formerly ConfigureServices)
                builder.Services.AddControllersWithViews();

                // Add session support
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(20);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });

                // Register application modules
                var mockData = bool.Parse(builder.Configuration["UseMockData"] ?? "false");
                var useCustomizationData = bool.Parse(builder.Configuration["UseCustomizationData"] ?? "false");
                builder.Services.AddApplicationModule(mockData);

                // Register database initializer
                builder.Services.AddScoped<IDatabaseInitializer<CatalogDBContext>, CatalogDBInitializer>();

                // Configure globalization settings
                builder.Services.Configure<RequestLocalizationOptions>(options =>
                {
                    var supportedCultures = new[]
                    {
                        new CultureInfo("en-US")
                    };

                    options.DefaultRequestCulture = new RequestCulture("en-US");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                });

                // Add logging
                builder.Services.AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline (formerly Configure method)
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                // Configure database
                ConfigureDatabase(app, mockData);

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                // Configure localization
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") },
                    SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US") }
                });

                // Add request logging middleware
                app.Use(async (context, next) =>
                {
                    // Create activity ID for request tracking
                    if (Trace.CorrelationManager.ActivityId == Guid.Empty)
                    {
                        Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                    }

                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogDebug($"WebApplication_BeginRequest: {context.Request.Path}, {context.Request.Headers["User-Agent"]}");

                    await next();
                });

                app.UseSession();

                app.UseRouting();

                app.UseAuthorization();

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }

            private static void ConfigureDatabase(WebApplication app, bool mockData)
            {
                if (!mockData)
                {
using (var scope = app.Services.CreateScope())
                    {
                        var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer<CatalogDBContext>>();
                        Database.SetInitializer(initializer);
                    }
                }
            }
        }

        public static class ServiceCollectionExtensions
        {
            public static IServiceCollection AddApplicationModule(this IServiceCollection services, bool useMockData)
            {
                // Register services based on the ApplicationModule from Autofac
                if (useMockData)
                {
                    // Register mock implementations
                    // Add your mock service registrations here
                }
                else
                {
                    // Register real implementations
                    // Add your real service registrations here
                }

                return services;
            }
        }

        // Add class to hold file configuration settings from web.config
        public static class FileSettings
        {
            public static string BasePath => ConfigurationManager.Configuration["files:BasePath"];
            public static string ServiceAccountId => ConfigurationManager.Configuration["files:ServiceAccountId"];
            public static string ServiceAccountDomain => ConfigurationManager.Configuration["files:ServiceAccountDomain"];
            public static string ServiceAccountPassword => ConfigurationManager.Configuration["files:ServiceAccountPassword"];
        }

        public class ConfigurationManager
        {
            public static IConfiguration Configuration { get; set; }
        }
    }