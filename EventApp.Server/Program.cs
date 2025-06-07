using EventApp.DI;
using EventApp.Helpers;
using EventApp.Middlewares;
using EventApp.Server.Helpers;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

DIContainer.RegisterDependency(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleWare>();

DatabaseManagementService.MigrationInitialisation(app, app.Environment.ContentRootPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads", "EventsImages")),
    RequestPath = "/Images"
});

app.UseRouting();
app.UseCors("AllowWithCredentials");

//app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
