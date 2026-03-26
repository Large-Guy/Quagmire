namespace Quagmire.Assets;

public abstract class Asset
{
    private string _path;
    private bool _runtime;

    public string Path => _path;
    public string Directory => System.IO.Path.GetDirectoryName(_path) != null ? System.IO.Path.GetDirectoryName(_path) + "/" : "";
    public bool IsRunTime => _runtime;

    public Asset(string path)
    {
        _path = path;
        _runtime = path.IsWhiteSpace();
    }
}