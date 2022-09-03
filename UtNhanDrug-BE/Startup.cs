using CorePush.Apple;
using CorePush.Google;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UtNhanDrug_BE.Configurations;
using UtNhanDrug_BE.Models.FcmNoti;
using UtNhanDrug_BE.Services.AuthenticationService;
using UtNhanDrug_BE.Services.FcmNotificationService;

namespace UtNhanDrug_BE
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
            // Add Firebase Services
            AddFireBaseAsync();

            services.AddCors();
            // register (Authentication - JWT) Module 
            services.RegisterSecurityModule(Configuration);

            services.AddControllers().AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            
            services.AddApiVersioning(x =>
            {
                x.DefaultApiVersion = new ApiVersion(1, 0);
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });

            // register (Swagger) Module
            services.RegisterSwaggerModule();

            services.AddScoped<IAuthenticationSvc, AuthenticationSvc>();

            services.AddTransient<INotificationService, NotificationService>();
            //register fcm service
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();

            // Register appsetting
            var appSettingsSection = Configuration.GetSection("FcmNotification");
            services.Configure<FcmNotificationSetting>(appSettingsSection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseApplicationSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseApplicationSecurity();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public virtual FirebaseApp AddFireBaseAsync()
        {
            // Get Current Path
            var currentDirectory = Directory.GetCurrentDirectory();
            // Path of firebase.json
            var jsonFirebasePath = Path.Combine(currentDirectory, "Cert", "firebase.json");
            // Initialize the default app
            var defaultApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(jsonFirebasePath)
            });
            return defaultApp;
        }
    }
}
