using DevRecord.Api.DTOs.Common;

namespace DevRecord.Api.DTOs.Entries;

public sealed record EntryQueryParameters : AcceptHeaderDto
{
    public string? Fields { get; init; }
}
