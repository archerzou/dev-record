namespace DevRecord.Api.Settings;

public sealed class CorsOptions
{
    public const string PolicyName = "DevRecordCorsPolicy";
    public const string SectionName = "Cors";

    public required string[] AllowedOrigins { get; init; }
}
