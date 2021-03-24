using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Infrastructure.Data;
using API.Helpers;
using API.Middlewares;
using API.Extensions;
using StackExchange.Redis;
using Infrastructure.Identity;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddApplicationServices();
            services.AddControllers();

            services.AddDbContext<StoreContext>(options => options.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AppIdentityDbContext>(options => 
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDbContext<AppIdentityDbContext>(options => 
            {
                options.UseSqlite(_configuration.GetConnectionString("IdentityConnection"));
            });
            services.AddIdentityServices(_configuration);


            services.AddSingleton<IConnectionMultiplexer>(config =>
                {
                    var configuration = ConfigurationOptions.Parse(_configuration.GetConnectionString("Redis"), true);
                    return ConnectionMultiplexer.Connect(configuration);
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"); });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
