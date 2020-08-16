using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TopSaloon.API.AutoMapperConfig;
using TopSaloon.API.Extensions;
using TopSaloon.Core;
using TopSaloon.Core.Managers;
using TopSaloon.DAL;
using TopSaloon.Entities.Models;

namespace TopSaloon.API
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

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=BOLT-PC20\\SQLEXPRESS;Database=TOPSALOON;User Id=sa;Password=A_12345;"));

            // services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=tcp:boltsql2020.database.windows.net,1433;Initial Catalog=TOPSALOON;Persist Security Info=False;User ID=boltadmin;Password=S3cur!ty;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));

            // services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer("Server=BOLT-PC15\\SQLEXPRESS;Initial Catalog=TOPSALOON;Persist Security Info=False;User ID=sa;Password=password;"));

            services.AddCors(options =>
                options.AddDefaultPolicy(builder =>
                    builder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<ApplicationUserManager>();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<UnitOfWork>();

            

            services.AddBusinessServices();

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseCors(policy => policy
           .AllowAnyHeader()
           .AllowAnyMethod()
           .WithOrigins("http://localhost:4200")
           .AllowCredentials());

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
