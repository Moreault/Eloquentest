namespace Eloquentest.Sample;

public interface IDummyService
{
    void DeleteFiles(IEnumerable<string> filenames);
}

[AutoInject]
public class DummyService : IDummyService
{
    private readonly IFile _file;
    private readonly IDirectory _directory;

    public DummyService(IFile file, IDirectory directory)
    {
        _file = file;
        _directory = directory;
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
}