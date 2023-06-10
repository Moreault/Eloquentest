namespace Eloquentest.Sample;

public record ComplexDummy
{
    public Guid Id { get; init; }
    public DateTimeOffset LastUpdate { get; init; }
    public string Description { get; init; } = string.Empty;
    public ComplexDummyChild Child { get; init; } = new();
}

public record ComplexDummyChild
{
    public ulong Id { get; init; }
    public DateTimeOffset LastUpdate { get; init; }
    public double Price { get; init; }
    public ImmutableList<ComplexDummyElement> Children { get; init; } = ImmutableList<ComplexDummyElement>.Empty;
    public IImmutableList<ComplexDummyElement> Interfaces { get; init; } = ImmutableList<ComplexDummyElement>.Empty;
}

public record ComplexDummyElement
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTimeOffset LastTimeWatched { get; init; }
}