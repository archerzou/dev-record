using DevRecord.Api.DTOs.Common;

namespace DevRecord.Api.DTOs.Tags;

public sealed record TagsCollectionDto : ICollectionResponse<TagDto>
{
    public List<TagDto> Items { get; init; }
}
