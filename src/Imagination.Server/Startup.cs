using System.Net;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Imagination.Configurations;
using Imagination.ImageHelper;
using Imagination.ImageHelper.FormatWrappers;
using Imagination.ServicesHandler;

namespace Imagination
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private IConfiguration Configuration { get; }
        public IConfigurationRoot _appConfiguration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenTelemetryTracing(builder => builder
                .SetResourceBuilder(ResourceBuilder
                    .CreateDefault()
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk()
                    .AddService("Imagination"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddJaegerExporter()
                .AddSource(Program.Telemetry.Name));

            services.AddScoped<IImageProcessor, ImageProcessor>();
            services.AddScoped<IFormatWrapperFactory, FormatWrapperFactory>();
            services.AddScoped<IRequestHandler, RequestHandler>();
            services.Configure<ImageConfiguration>(_appConfiguration.GetSection("Image:Configuration"));

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {}

            app.UseExceptionHandler(
                    options => options.Run(
                        async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                            var ex = context.Features.Get<IExceptionHandlerPathFeature>();

                            if (ex != null) { 
                                await context.Response.WriteAsJsonAsync(ex.Error.Message);

                                using var activity = Program.Telemetry.StartActivity(ex.Error.Message);
                                    activity?.SetStatus(Status.Error);
                            }
                        }));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
