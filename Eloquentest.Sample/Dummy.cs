namespace Eloquentest.Sample;

public interface IDummy
{

}

public record Dummy : IDummy
{
    public int Id { get; init; }
    public IReadOnlyList<SubDummy> Subs { get; init; } = Array.Empty<SubDummy>();
}