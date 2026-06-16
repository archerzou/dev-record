using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using WireMock.Server;

namespace DevRecord.IntegrationTests.Infrastructure;

public sealed class DevRecordWebAppFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:17.2")
        .WithDatabase("devrecord")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private WireMockServer _wireMockServer;

    public WireMockServer GetWireMockServer() => _wireMockServer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Database", _postgresContainer.GetConnectionString());
        builder.UseSetting("GitHub:BaseUrl", _wireMockServer.Urls[0]);
        
        // Valid 32-byte Base64 key for AES-256; replaces the placeholder in appsettings.Development.json
        // which is not valid Base64 and has no user-secrets override in CI
        builder.UseSetting("Encryption:Key", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=");

        builder.ConfigureServices(services =>
        {
            // Remove Quartz hosted service to prevent LoggerFactory disposal conflict
            // during WebApplicationFactory startup (Quartz XML processor logs during Host.StartAsync)
            var quartzHostedServices = services
                .Where(d => d.ServiceType == typeof(IHostedService) &&
                            d.ImplementationType?.Assembly.GetName().Name?.Contains("Quartz") == true)
                .ToList();

            foreach (ServiceDescriptor descriptor in quartzHostedServices)
            {
                services.Remove(descriptor);
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        _wireMockServer = WireMockServer.Start();
    }

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();

    public override async ValueTask DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        _wireMockServer?.Stop();
        await base.DisposeAsync();
    }
}
