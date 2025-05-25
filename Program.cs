using EventApp.DI;
using EventApp.Helpers;
using EventApp.Middlewares;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

DIContainer.RegisterDependency(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleWare>();

using (var scope = app.Services.CreateScope())
    await DbInitializer.SeedUsersAndRolesAsync(scope.ServiceProvider);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/Resources"
});

app.UseHttpsRedirection();
app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
