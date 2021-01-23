using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NationalParkAPI.Data;
using NationalParkAPI.Mapper;
using NationalParkAPI.Repository;
using NationalParkAPI.Repository.IRepository;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NationalParkAPI
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddAutoMapper(typeof(Mappings));
            services.AddApiVersioning(option =>
            {
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(options => options.GroupNameFormat = "'v'VVV");
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen();
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("NationalParkAPI", new OpenApiInfo
            //     {
            //         Title = "NationalParkAPI", 
            //         Version = "1",
            //         Description = "First api from Tri",
            //         Contact = new OpenApiContact()
            //         {
            //             Email = "Nmtri3108@gmail.com",
            //             Name ="Nguyen Minh Tri"
            //         }
            //     });
            //     // c.SwaggerDoc("NationalParkAPITrails", new OpenApiInfo
            //     // {
            //     //     Title = "NationalParkAPI Trail", 
            //     //     Version = "1",
            //     //     Description = "First api from Tri",
            //     //     Contact = new OpenApiContact()
            //     //     {
            //     //         Email = "Nmtri3108@gmail.com",
            //     //         Name ="Nguyen Minh Tri"
            //     //     }
            //     // });
            //
            //     var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //     var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
            //     c.IncludeXmlComments(cmlCommentFullPath);
            // });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    foreach (var desc in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                            desc.GroupName.ToUpperInvariant());
                        options.RoutePrefix = "";
                    }
                });
                // app.UseSwaggerUI(c =>
                // {
                //     c.SwaggerEndpoint("/swagger/NationalParkAPI/swagger.json", "NationalParkAPI");
                //     // c.SwaggerEndpoint("/swagger/NationalParkAPITrails/swagger.json", "NationalParkAPI Trail");
                //     c.RoutePrefix = "";
                // });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}