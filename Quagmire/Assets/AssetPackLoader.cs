namespace Quagmire.Assets;

public class AssetPackLoader : IAssetLoader
{
    public Asset Load(AssetPack pack, string path)
    {
        return new AssetPack(path, pack);
    }
}