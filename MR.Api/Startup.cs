using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using MR.DataAccessLayer.Interfaces;
using MR.DataAccessLayer.Repositories;
using MR.LogicLayer.Interfaces;
using MR.LogicLayer.Services;
using MR.DataAccessLayer.Data;

namespace MR.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MR.Api", Version = "v1" });
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // DbContext Register
            services.AddDbContext<DataAccessLayer.Context.MovieReviewerContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // DI Configuration Logic Layer
            services.AddScoped<ICommentService, CommentService>();

            // DI Configuration Data Layer
            services.AddScoped<ICommentRepository, CommentRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MR.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
