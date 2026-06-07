using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DevRecord.Api.DTOs.Habits;
using DevRecord.Api.Entities;
using DevRecord.IntegrationTests.Infrastructure;

namespace DevRecord.IntegrationTests.Tests;
public sealed class HabitsTests(DevRecordWebAppFactory factory) : IntegrationTestFixture(factory)
{
    [Fact]
    public async Task CreateHabit_ShouldSucceed_WithValidParameters()
    {
        // Arrange
        var dto = new CreateHabitDto
        {
            Name = "Read Books",
            Description = "Read technical books to improve skills",
            Type = HabitType.Measurable,
            Frequency = new FrequencyDto
            {
                Type = FrequencyType.Daily,
                TimesPerPeriod = 1
            },
            Target = new TargetDto
            {
                Value = 30,
                Unit = "pages"
            }
        };

        HttpClient client = await CreateAuthenticatedClientAsync();

        // Act
        HttpResponseMessage response = await client.PostAsJsonAsync(Routes.Habits.Create, dto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<HabitDto>());
    }
}
