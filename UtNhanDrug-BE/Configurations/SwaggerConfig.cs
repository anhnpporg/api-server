using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UtNhanDrug_BE.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection RegisterSwaggerModule(this IServiceCollection services)
        {

            services.AddSwaggerGen(c =>
            {
                // Set Description Swagger
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UtNhanDrug",
                    Version = "v1",
                    Description = "\nUtNhanDrug API - \n" +
                    "\n Admin : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwicm9sZVVzZXJJZCI6IjEiLCJyb2xlIjoiTUFOQUdFUiIsIm5iZiI6MTY2OTEwMjMzOCwiZXhwIjoxNjcwMzExOTM4LCJpYXQiOjE2NjkxMDIzMzh9.RNOmA5O8Lh_WkVD-CdK8XB_FMjcTFni55ohWz4qIre8\n"
                    + "\n Staff : eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImN0eSI6IkpXVCJ9.eyJ1c2VySWQiOiIxNCIsInJvbGVVc2VySWQiOiI3Iiwicm9sZSI6IlNUQUZGIiwibmJmIjoxNjY5MTAyMzcyLCJleHAiOjE2NzAzMTE5NzIsImlhdCI6MTY2OTEwMjM3Mn0.Zk4cOBZNp7qSfYKomchMgZ1Zoe9zdpt6OdafmZht8CA"
                    ,
                    Contact = new OpenApiContact()
                    {
                        Name = "UtNhanDrug"
                    }

                });
                //c.OrderActionsBy((apiDesc) => $"{apiDesc.RelativePath}");

                c.DescribeAllParametersInCamelCase();
                // Set the comments path for the Swagger JSON and UI.
/*                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);*/

                // c.SchemaFilter<EnumSchemaFilter>();

                // Set Authorize box to swagger
                var jwtSecuriyScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your token on textbox below!\n Name = Authentication",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecuriyScheme.Reference.Id, jwtSecuriyScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {jwtSecuriyScheme, Array.Empty<string>()}
                });


            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }

        public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "UtNhanDrug.WebAPI v1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }

    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                model.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(n => model.Enum.Add(new OpenApiString(n)));
            }
        }
    }
}
