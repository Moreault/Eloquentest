namespace Eloquentest.Sample;

public interface IDummyService
{
    void DeleteFiles(IEnumerable<string> filenames);
    IReadOnlyList<SubDummy> GetSubs(int dummyId);
    void DeleteFile(string filename);
    void DeleteDirectory(string directory);
}

[AutoInject]
public class DummyService : IDummyService
{
    private readonly IFile _file;
    private readonly IDirectory _directory;
    private readonly IOtherDummyService _otherDummyService;

    public DateTime SomeDate { get; init; }

    public DummyService(IFile file, IDirectory directory, IOtherDummyService otherDummyService)
    {
        _file = file;
        _directory = directory;
        _otherDummyService = otherDummyService;
    }

    public void DeleteFiles(IEnumerable<string> filenames)
    {
        if (filenames == null) throw new ArgumentNullException(nameof(filenames));

        foreach (var file in filenames)
        {
            var directory = Path.GetDirectoryName(file)!;
            if (!_directory.Exists(directory)) throw new Exception("Directory doesn't even exist!");

            _file.Delete(file);
        }
    }

    public IReadOnlyList<SubDummy> GetSubs(int dummyId) => _otherDummyService.GetDummy(dummyId).Subs;

    public void DeleteFile(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename));
        _file.Delete(filename);
    }

    public void DeleteDirectory(string directory)
    {
        if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException(nameof(directory));
        _directory.DeleteNonRecursively(directory);
    }
}