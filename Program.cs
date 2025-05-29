using EventApp.DI;
using EventApp.Helpers;
using EventApp.Middlewares;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

DIContainer.RegisterDependency(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleWare>();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.SeedUsersAndRolesAsync(scope.ServiceProvider);

    string path = Path.Combine(app.Environment.ContentRootPath, "Uploads");
    if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads", "EventsImages")),
    RequestPath = "/Images"
});

app.UseRouting();
app.UseCors("AllowWithCredentials");

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
