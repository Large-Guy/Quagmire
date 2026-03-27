using Quagmire.Assets;

namespace Quagmire.Rendering;

public class FontLoader : IAssetLoader
{
    public Asset Load(AssetPack pack, string path)
    {
        return new Font(path);
    }
}