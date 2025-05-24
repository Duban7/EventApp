using EventApp.DI;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

DIContainer.RegisterDependency(builder.Services, builder.Configuration);

var app = builder.Build();

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
