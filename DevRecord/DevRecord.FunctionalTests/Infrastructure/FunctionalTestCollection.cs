namespace DevRecord.FunctionalTests.Infrastructure;

[CollectionDefinition(nameof(FunctionalTestCollection))]
public sealed class FunctionalTestCollection : ICollectionFixture<DevRecordWebAppFactory>;
