namespace Quagmire.Assets;

public interface IAssetLoader
{
    public Asset Load(AssetPack pack, string path);
}