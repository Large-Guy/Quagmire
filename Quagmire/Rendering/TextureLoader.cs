using Quagmire.Assets;

namespace Quagmire.Rendering;

public class TextureLoader : IAssetLoader
{
    public Asset Load(AssetPack pack, string path)
    {
        return new Texture2D(path);
    }
}