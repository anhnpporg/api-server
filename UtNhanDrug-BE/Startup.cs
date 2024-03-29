using CorePush.Apple;
using CorePush.Google;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using System.IO;
using UtNhanDrug_BE.Configurations;
using UtNhanDrug_BE.Entities;
using UtNhanDrug_BE.Models.FcmNoti;
using UtNhanDrug_BE.Models.RoleModel;
using UtNhanDrug_BE.Services.ActiveSubstanceService;
using UtNhanDrug_BE.Services.AuthenticationService;
using UtNhanDrug_BE.Services.BrandService;
using UtNhanDrug_BE.Services.BatchService;
using UtNhanDrug_BE.Services.DiseaseService;
using UtNhanDrug_BE.Services.EmailSenderService;
using UtNhanDrug_BE.Services.FcmNotificationService;
using UtNhanDrug_BE.Services.GoodsReceiptNoteService;
using UtNhanDrug_BE.Services.ManagerService;
using UtNhanDrug_BE.Services.ProductActiveSubstanceService;
using UtNhanDrug_BE.Services.ProductService;
using UtNhanDrug_BE.Services.ProductUnitService;
using UtNhanDrug_BE.Services.SamplePrescriptionDetailService;
using UtNhanDrug_BE.Services.SamplePrescriptionService;
using UtNhanDrug_BE.Services.ShelfService;
using UtNhanDrug_BE.Services.SupplierService;
using UtNhanDrug_BE.Services.InvoiceService;
using UtNhanDrug_BE.Services.DashBoardService;
using UtNhanDrug_BE.Services.ProfileUserService;
using UtNhanDrug_BE.Services.HandlerService;
using UtNhanDrug_BE.Services.PointService;
using UtNhanDrug_BE.Services.InventorySystemReportsService;
using Quartz;
using UtNhanDrug_BE.Services.ScheduleService;
using System;

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
        [System.Obsolete]
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

            //add scope

            services.AddScoped<IAuthenticationSvc, AuthenticationSvc>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IInvoiceSvc, InvoiceSvc>();
            services.AddTransient<IUserSvc, UserSvc>();
            services.AddTransient<IProfileUserSvc, ProfileUserSvc>();
            services.AddTransient<IPointSvc, PointSvc>();
            services.AddTransient<IInventoryReport, InventoryReport>();
            services.AddTransient<SchedulerService>();
            services.AddTransient<IBrandSvc, BrandSvc>();
            services.AddTransient<IShelfSvc, ShelfSvc>();
            services.AddTransient<IActiveSubstanceSvc, ActiveSubstanceSvc>();
            services.AddTransient<IHandlerSvc,HandlerSvc>();
            services.AddTransient<IDashBoardSvc, DashBoardSvc>();
            services.AddTransient<IPASSvc, PASSvc>();
            services.AddTransient<IProductSvc, ProductSvc>();
            services.AddTransient<ISamplePrescriptionSvc, SamplePrescriptionSvc>();
            services.AddTransient<ISamplePrescriptionDetailSvc, SamplePrescriptionDetailSvc>();
            services.AddTransient<IDiseaseSvc, DiseaseSvc>();
            services.AddTransient<ISenderService, SenderService>();
            services.AddTransient<IGRNSvc, GRNSvc>();
            services.AddTransient<IProductUnitPriceSvc, ProductUnitPriceSvc>();
            services.AddTransient<IBatchSvc, BatchSvc>();
            services.AddTransient<ISupplierSvc, SupplierSvc>();
            services.AddTransient<RoleType>();

            services.AddDbContext<ut_nhan_drug_store_databaseContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("utNhanDrug")));

            //register fcm service
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();

            // Register appsetting
            var appSettingsSection = Configuration.GetSection("FcmNotification");
            services.Configure<FcmNotificationSetting>(appSettingsSection);

            services.AddQuartz(q =>
            {
                // base Quartz scheduler, job and trigger configuration
                q.UseMicrosoftDependencyInjectionScopedJobFactory();
                // Just use the name of your job that you created in the Jobs folder.

                var jobKey1 = new JobKey("CheckExpiryBatch3MonthSchedule");
                q.AddJob<CheckExpiryBatch3MonthSchedule>(opts => opts.WithIdentity(jobKey1));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey1)
                    .WithIdentity("CheckExpiryBatch3MonthSchedule-trigger")
                    .WithCronSchedule("0 11 1 * * ?", x => x
                            .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))

                );

                var jobKey = new JobKey("SendEmailJob");
                q.AddJob<SchedulerService>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("SendEmailJob-trigger")
                    .WithCronSchedule("0 0 0 * * ?", x => x
                            .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
                );


            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
                //.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

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
