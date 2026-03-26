using System.Text.Json;
using Quagmire.Assets;
using Quagmire.Geometry;

namespace Quagmire.Rendering;

public class TextureRegionLoader : IAssetLoader
{
    public Asset Load(AssetPack pack, string path)
    {
        return new TextureRegion(pack, path);
    }
}