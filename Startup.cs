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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NationalParkAPI.Data;
using NationalParkAPI.Mapper;
using NationalParkAPI.Repository;
using NationalParkAPI.Repository.IRepository;

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
            services.AddAutoMapper(typeof(Mappings));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("NationalParkAPI", new OpenApiInfo {Title = "NationalParkAPI", Version = "v1"});
                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var cmlCommentFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                c.IncludeXmlComments(cmlCommentFullPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(); 
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/NationalParkAPI/swagger.json", "NationalParkAPI v1");
                    c.RoutePrefix = "";
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}