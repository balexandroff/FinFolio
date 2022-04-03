using FinFolio.Data;
using FinFolio.Core.Entities;
using FinFolio.Services.Implementaiton;
using FinFolio.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using FinFolio.Core.Interfaces;
using FinFolio.Data.Repositories;
using System.Reflection;
using System.Linq;

namespace FinFolio
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
            services.RegisterRepositories();
            services.RegisterServices();

            services.AddDbContext<FinFolioContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<FinFolioContext>();
            services.AddRazorPages();
            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinFolio.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinFolio.API v1"));

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
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            var repoTypes = Assembly.Load(typeof(BaseRepository).Assembly.GetName())
                          .GetTypes()
                          .Where(x => !string.IsNullOrEmpty(x.Namespace))
                          .Where(x => x.IsClass)
                          .Where(x => !x.IsAbstract)
                          .Where(x => typeof(IRepository).IsAssignableFrom(x))
                          .Select(x => new
                          {
                              Interface = x.GetInterface($"I{x.Name}"),
                              Implementation = x
                          })
                          .ToList();

            foreach (var repoType in repoTypes)
            {
                var a = repoType.Interface;
                var b = repoType.Implementation;
                services.AddTransient(repoType.Interface, repoType.Implementation);
            }

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            var serviceTypes = Assembly.Load(typeof(StockService).Assembly.GetName())
                          .GetTypes()
                          .Where(x => !string.IsNullOrEmpty(x.Namespace))
                          .Where(x => x.IsClass)
                          .Where(x => !x.IsAbstract)
                          .Where(x => typeof(IService).IsAssignableFrom(x))
                          .Select(x => new
                          {
                              Interface = x.GetInterface($"I{x.Name}"),
                              Implementation = x
                          })
                          .ToList();

            foreach (var serviceType in serviceTypes)
            {
                services.AddTransient(serviceType.Interface, serviceType.Implementation);
            }

            return services;
        }
    }
}
