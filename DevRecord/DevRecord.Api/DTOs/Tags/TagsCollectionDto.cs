using DevRecord.Api.DTOs.Common;
using Newtonsoft.Json;

namespace DevRecord.Api.DTOs.Tags;

public sealed record TagsCollectionDto : ICollectionResponse<TagDto>, ILinksResponse
{
    public List<TagDto> Items { get; init; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<LinkDto> Links { get; set; }
}
