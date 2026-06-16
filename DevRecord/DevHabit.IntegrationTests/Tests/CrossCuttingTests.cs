using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using DevRecord.Api.DTOs.Habits;
using DevRecord.Api.Entities;
using DevRecord.Api.Services;
using DevRecord.IntegrationTests.Infrastructure;

namespace DevRecord.IntegrationTests.Tests;

public sealed class CrossCuttingTests(DevRecordWebAppFactory factory) : IntegrationTestFixture(factory)
{
    public static TheoryData<string> ProtectedEndpoints =>
#pragma warning disable CA1825
    [
        Routes.Habits.GetAll,
        Routes.Entries.GetAll,
        Routes.Tags.GetAll,
        Routes.GitHub.GetProfile,
        Routes.EntryImports.GetAll
    ];
#pragma warning restore CA1825

    public static TheoryData<string> MediaTypes =>
#pragma warning disable CA1825
    [
        MediaTypeNames.Application.Json,
        CustomMediaTypeNames.Application.HateoasJson
    ];
#pragma warning restore CA1825

    [Theory]
    [MemberData(nameof(ProtectedEndpoints))]
    public async Task Endpoints_ShouldRequireAuthentication(string route)
    {
        // Arrange
        HttpClient client = CreateClient();

        // Act
        HttpResponseMessage response = await client.GetAsync(route);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.First().Scheme);
    }

    [Fact]
    public async Task Endpoints_ShouldEnforceResourceOwnership()
    {
        // Arrange
        HttpClient client1 = await CreateAuthenticatedClientAsync("user1@test.com", forceNewClient: true);
        HttpClient client2 = await CreateAuthenticatedClientAsync("user2@test.com", forceNewClient: true);

        // Create a habit with user1
        CreateHabitDto habitDto = TestData.Habits.CreateReadingHabit();
        HttpResponseMessage createResponse = await client1.PostAsJsonAsync(Routes.Habits.Create, habitDto);
        HabitDto? habit = await createResponse.Content.ReadFromJsonAsync<HabitDto>();
        Assert.NotNull(habit);

        // Act & Assert - Try to access with user2
        HttpResponseMessage response = await client2.GetAsync(Routes.Habits.GetById(habit.Id));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MediaTypes))]
    public async Task Api_ShouldSupportContentNegotiation(string mediaType)
    {
        // Arrange
        HttpClient client = await CreateAuthenticatedClientAsync();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

        // Act
        HttpResponseMessage response = await client.GetAsync(Routes.Habits.GetAll);

        // Assert
        Assert.Equal(mediaType, response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Api_ShouldReturnProblemDetails_OnError()
    {
        // Arrange
        HttpClient client = await CreateAuthenticatedClientAsync();

        // Create an invalid habit (missing required fields)
        var invalidDto = new CreateHabitDto
        {
            Name = string.Empty, // Invalid - name is required
            Type = HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 1
            },
            Target = new TargetDto
            {
                Value = 1,
                Unit = "tasks"
            }
        };

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(Routes.Habits.Create, invalidDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.ProblemJson, response.Content.Headers.ContentType?.MediaType);

        Dictionary<string, object>? problemDetails = await response.Content
            .ReadFromJsonAsync<Dictionary<string, object>>();
        Assert.NotNull(problemDetails);
        Assert.True(problemDetails.ContainsKey("type"));
        Assert.True(problemDetails.ContainsKey("title"));
        Assert.True(problemDetails.ContainsKey("status"));
        Assert.True(problemDetails.ContainsKey("requestId"));
    }
}
