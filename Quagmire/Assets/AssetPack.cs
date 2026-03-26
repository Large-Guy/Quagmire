using System.Reflection;
using System.Text.Json;

namespace Quagmire.Assets;

public class AssetPack : Asset
{
    private static readonly Dictionary<string, IAssetLoader> Loaders = [];
    private class AssetInfo
    {
        public class Data
        {
            public string Type { get; set; }
            public string Path { get; set; }
        }
        public string Path { get; set; }
        public string Loader { get; set; }
        public Data? Metadata { get; set; } = null;
    }

    private AssetPack? _parent;
    private Dictionary<string, AssetInfo> _assetsRegistry = [];
    private readonly Dictionary<string, Asset> _loadedAssets = [];

    public AssetPack? Parent => _parent;
    public AssetPack Root => GetRoot();

    private AssetPack GetRoot()
    {
        return _parent?.GetRoot() ?? this;
    }
    
    public AssetPack(string packPath, AssetPack? parent = null) : base(packPath)
    {
        _parent = parent;
        try
        {
            var content = File.ReadAllText(packPath);
            _assetsRegistry = JsonSerializer.Deserialize<Dictionary<string,AssetInfo>>(content) ?? 
                              throw new Exception("Unable to load assets properly");
        }
        catch (FileNotFoundException)
        {
            throw new Exception($"Error: The file at {packPath} was not found.");
        }
        catch (JsonException ex)
        {
            throw new Exception($"Error deserializing JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new Exception($"An unexpected error occurred: {ex.Message}");
        }
    }

    public bool Has<T>(string name) where T : Asset
    {
        return _assetsRegistry.ContainsKey(name);
    }
    
    public T Load<T>(string name) where T : Asset
    {
        if (name.StartsWith('!'))
        {
            return Root.Load<T>(name[1..]);
        }

        if (name.StartsWith('^'))
        {
            return Parent?.Load<T>(name[1..]) ?? throw new Exception($"This asset pack has no parent {Path}");
        }
        if (name.Contains('.'))
        {
            var path = name.Split('.');
            var current = this;
            for (var i = 0; i < path.Length - 1; i++)
            {
                current = current.Load<AssetPack>(path[i]);
            }

            return current.Load<T>(path.Last());
        }
        
        if (_loadedAssets.TryGetValue(name, out var asset))
        {
            return asset as T ?? throw new Exception($"Asset is not of type: {typeof(T).Name}");
        }

        if (!_assetsRegistry.TryGetValue(name, out var assetInfo)) 
            throw new Exception($"Invalid asset '{name}'");
        
        if (!Loaders.TryGetValue(assetInfo.Loader, out var loader))
        {
            var type = Type.GetType(assetInfo.Loader) 
                       ?? AppDomain.CurrentDomain.GetAssemblies()
                           .Select(a => a.GetType(assetInfo.Loader))
                           .FirstOrDefault(t => t != null)
                       ?? throw new Exception($"Unable to find Asset Loader: {assetInfo.Loader}");
                
            loader = (IAssetLoader?)Activator.CreateInstance(type) ?? throw new Exception("Unable to create asset loader");
            Loaders[assetInfo.Loader] = loader;
        }

        if (loader == null) throw new Exception("Unable to get asset loader");

        var loaded = loader.Load(this, System.IO.Path.Combine(Directory, assetInfo.Path));
        if (loaded == null || loaded is not T loadedAsset)
        {
            throw new Exception($"Unable to load asset: '{name}'");
        }

        _loadedAssets[name] = loadedAsset;
        return loadedAsset;

    }
}