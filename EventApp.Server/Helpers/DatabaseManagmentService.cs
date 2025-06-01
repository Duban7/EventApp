using Data.Context;
using EventApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EventApp.Server.Helpers
{
    public static class DatabaseManagementService
    {
        public async static void MigrationInitialisation(IApplicationBuilder app, string contentRootPath)
        {
            string path = Path.Combine(contentRootPath, "Uploads", "EventsImages");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<EventAppDbContext>();

                if (!context.Database.GetService<IRelationalDatabaseCreator>().Exists())
                {
                    context.Database.Migrate();
                }


                await DbInitializer.SeedUsersAndRolesAsync(serviceScope.ServiceProvider);

            }
        }
    }
}
