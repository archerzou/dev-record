namespace DevRecord.Api.DTOs.EntryImports;

public sealed record CreateEntryImportJobDto
{
    public required IFormFile File { get; init; }
}

