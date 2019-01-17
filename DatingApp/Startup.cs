using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DatingApp
{
    public class Startup
    {
        private Process _npmProcess;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsDevelopment())
            {
                try
                {
                    // Start the ng serve process using npm start
                    _npmProcess = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/C npm start",
                            UseShellExecute = false
                        }
                    };
                    // Start process
                    _npmProcess.Start();
                    // Register the application shutdown event
                    var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
                    applicationLifetime.ApplicationStopping.Register(OnShutDown);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseMvc();
        }

        private void OnShutDown()
        {
            if (_npmProcess != null)
            {
                try
                {
                    Console.WriteLine($"Killing process npm process ( {_npmProcess.StartInfo.FileName} {_npmProcess.StartInfo.Arguments} )");
                    _npmProcess.Kill();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to Kill npm process ( {_npmProcess.StartInfo.FileName} {_npmProcess.StartInfo.Arguments} )");
                    Console.WriteLine($"Exception: {ex}");
                }
            }
        }
    }
}
