// Startup.cs
using AiWeb.Models; // ← pridať ak treba
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AiWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
				    services.AddMemoryCache(); // 💾 Tu sa pridáva podpora pre cache
						//services.AddSingleton<WebsiteCache>(); // ← správne generické volanie				    
						services.AddScoped<WebsiteCache>();
            services.AddCors(options =>
            {
            	
                options.AddPolicy("AllowReact",
                    builder => builder.WithOrigins("http://localhost:5173", "http://frontend")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
                await next.Invoke();
            });

            app.UseCors("AllowReact");
            app.UseStaticFiles(); // musí byť pred app.UseRouting()
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}