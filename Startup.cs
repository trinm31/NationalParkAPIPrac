using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
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
            services.AddCors();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();
            services.AddScoped<ITrailRepository, TrailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
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
            var appsettingSection = Configuration.GetSection("Appsettings");
            
            services.Configure<Appsettings>(appsettingSection);
            var appsetting = appsettingSection.Get<Appsettings>();
            var key = Encoding.ASCII.GetBytes(appsetting.Secret);
            
            
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x=> {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
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
            app.UseCors(x=> x 
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}