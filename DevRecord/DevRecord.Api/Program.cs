using DevRecord.Api;
using DevRecord.Api.Extensions;
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


if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();

    await app.ApplyMigrationsAsync();

    await app.SeedInitialDataAsync();
}

app.UseExceptionHandler();

app.UseCors(CorsOptions.PolicyName);


app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseUserContextEnrichment();

app.MapControllers();
//app.UseETag();

await app.RunAsync();

public partial class Program;
