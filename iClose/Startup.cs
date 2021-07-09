using iClose.Models;
using iCloseAdmin.Shapes;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.DisplayManagement.Descriptors;
using OrchardCore.Email;
using OrchardCore.Email.Services;


namespace iClose {
    public class Startup {
        
        public Startup(IConfiguration configuration) {

            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<EmailConfigurationOption>(Configuration.GetSection("EmailConfiguration"));
            services.AddScoped<IShapeTableProvider, HtmlMediaShapes>();
            services.AddScoped<ISmtpService, SmtpService>();
            services.AddOrchardCms();
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IHostEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseOrchardCore();
        }
    }
}
