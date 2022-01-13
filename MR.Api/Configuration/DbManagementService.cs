using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MR.DataAccessLayer.Context;

namespace MR.Api.Configuration
{
    public static class DbManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                MovieReviewerContext context = serviceScope.ServiceProvider.GetService<MovieReviewerContext>();
                string connectionstring = context.Database.GetConnectionString();
                context.Database.EnsureCreated();
                context.Database.Migrate();
            }
        }
    }
}