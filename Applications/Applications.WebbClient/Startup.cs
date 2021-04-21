using System;
using Core.ApplicationServices;
using Core.Domain.Repositories;
using Core.Domain.Services.External;
using Core.Domain.Services.Internal;
using Core.Domain.Services.Internal.Interfaces;
using Core.Infrastructure.DataAccess.EfCoreDataAccess;
using Core.Infrastructure.Services.RakicRaiffeisenBrosBankService.Mock;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Applications.WebbClient
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
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            AdminRoleHelper.AdminPass = Configuration["adminPass"];

            // DbContext, UnitOfWork and RequestValidation services
            services.AddDbContextPool<CoreEfCoreDbContext>(options =>
            {
                string connString = Configuration["Query:ConnectionString"];
                options.UseSqlServer(connString);
            });

            services.AddScoped<ICoreUnitOfWork, CoreEfCoreUnitOfWork>();
            services.AddScoped<IBankService, RakicRaiffeisenBrosBankService>();

            services.AddSingleton<IPassService, PassService>((serviceProvider) =>
            {
                string pass_min = Configuration["passConfig:minPassConfig"];
                string pass_max = Configuration["passConfig:maxPassConfig"];

                return new PassService(pass_min, pass_max);
            });

            services.AddScoped((IServiceProvider serviceProvider) =>
            {
                return new WalletService(
                    serviceProvider.GetRequiredService<ICoreUnitOfWork>(),
                    serviceProvider.GetRequiredService<IBankService>(),
                    serviceProvider.GetRequiredService<IPassService>());
            });

            services.AddScoped((IServiceProvider serviceProvider) =>
            {
                return new TransactionService(serviceProvider.GetRequiredService<ICoreUnitOfWork>());
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
