using DevRecord.Api;
using DevRecord.Api.Extensions;
using DevRecord.Api.Middleware;
using DevRecord.Api.Settings;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices()
    .AddBackgroundJobs()
    .AddCorsPolicy()
    .AddRateLimiting();


WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();

    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);


app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

//app.UseMiddleware<ETagMiddleware>();

app.MapControllers();

await app.RunAsync();
