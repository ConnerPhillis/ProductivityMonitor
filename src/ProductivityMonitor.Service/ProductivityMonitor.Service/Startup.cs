using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using ProductivityMonitor.Service.Data;
using ProductivityMonitor.Service.HostedServices;
using ProductivityMonitor.Service.Services.Collection.ApplicationService;
using ProductivityMonitor.Service.Services.Collection.InputService;
using ProductivityMonitor.Service.Services.Collection.MousePositionService;

namespace ProductivityMonitor.Service
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
            services.AddControllersWithViews()
               .AddRazorRuntimeCompilation();

            services.AddHostedService<InputMonitorWorkerService>();
            services.AddHostedService<ApplicationMonitorService>();
            services.AddHostedService<MouseTrackingWorkerService>();

            services.AddSingleton<IApplicationService>(new WindowsApplicationService());

            services.AddSingleton<IInputMonitorService>(new WindowsInputMonitorService());

            services.AddSingleton<IMousePositionService>(new WindowsMousePositionService());

            services.AddDbContext<ProductivityMonitorDbContext>(
                options => options.UseSqlite(
                    @"Data Source=E:\Databases\ProductivityMonitorSvc.db"));

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1",
                        new OpenApiInfo {Title = "Productivity Service API", Version = "V1"});
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app.ApplicationServices);

            app.UseSwagger();
            app.UseSwaggerUI(
                c => { c.SwaggerEndpoint("/documentation/v1/swagger.json", "Productivity API V1"); });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Dashboard}/{action=Index}/{id?}");
                });
        }

        private void InitializeDatabase(IServiceProvider applicationService)
        {
            using var scope = applicationService.CreateScope();

            scope.ServiceProvider.GetRequiredService<ProductivityMonitorDbContext>()
               .Database.EnsureCreated();
        }
    }
}