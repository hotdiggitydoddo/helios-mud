using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Helios.Web.Services;
using Helios.Data;
using Helios.Domain.Models;
using Helios.Domain.Contracts;
using WebSocketManager;
using System;
using Helios.Engine;
//using Helios.Engine.Factories;
using Helios.Engine.UI;
using Helios.Engine.Factories;

namespace Helios.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<HeliosDbContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\ProjectsV13;Initial Catalog=HeliosDb;Integrated Security=True;Persist Security Info=False");
                //options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<User, IdentityRole<int>>(
                o =>
                {
                    o.Password.RequiredLength = 8;
                    o.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<HeliosDbContext, int>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddCors();
            services.AddWebSocketManager();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IRepository<Account>, Repository<Account>>();
            services.AddTransient<IRepository<Entity>, Repository<Entity>>();
            services.AddTransient<IRepository<Trait>, Repository<Trait>>();
            services.AddTransient<IEntityFactory, EntityFactory>();
            services.AddTransient<IOutputFormatter, OutputHtml>();
          //  services.AddTransient<IEntityService, EntityService>();
          //  services.AddTransient<IEntityManager, EntityManager>();
            
            services.AddHeliosGame();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCors(builder =>
            {
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.AllowAnyOrigin();
            });

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseWebSockets();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseHelios(serviceProvider.GetService<Game>());
            app.MapWebSocketManager("/io", serviceProvider.GetService<GameMessageHandler>());
        }
    }
}
