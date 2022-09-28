namespace Eloquentest.Sample;

public interface IOtherDummyService
{
    Dummy GetDummy(int id);
}

[AutoInject]
public class OtherDummyService : IOtherDummyService
{
    public Dummy GetDummy(int id) => new()
    {
        Id = id,
        Subs = new List<SubDummy>
        { 
            new()
            {
                Id = 14,
            }
        }
    };
}