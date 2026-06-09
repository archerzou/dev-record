using DevRecord.Api.DTOs.Common;

namespace DevRecord.Api.DTOs.Habits;

public sealed record HabitQueryParameters : AcceptHeaderDto
{
    public string? Fields { get; init; }
}
