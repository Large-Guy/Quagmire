using System.Text.Json;
using Quagmire.Assets;
using Quagmire.Geometry;
using SDL3;

namespace Quagmire.Rendering;

public class TextureRegion : Texture
{
    private class TextureRegionMetadata
    {
        public string Texture { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float W { get; set; }
        public float H { get; set; }
    }
    
    public TextureRegion(AssetPack pack, string path) : base(path)
    {
        try
        {
            var content = File.ReadAllText(path);
            var metadata = JsonSerializer.Deserialize<TextureRegionMetadata>(content) ?? 
                           throw new Exception("Unable to load assets properly");
            InternalTexture = pack.Load<Texture>(metadata.Texture).GetTexture();
            Source = new Rectangle(new Point(metadata.X, metadata.Y), new Point(metadata.W, metadata.H));
        }
        catch (FileNotFoundException)
        {
            throw new Exception($"Error: The file at {path} was not found.");
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
    public TextureRegion(Texture? texture, Rectangle? region = null) : base("")
    {
        InternalTexture = texture?.GetTexture() ?? IntPtr.Zero;
        var w = 0f;
        var h = 0f;
        if(InternalTexture != IntPtr.Zero)
            SDL.GetTextureSize(InternalTexture, out w, out h);
        var position = new Point(texture?.GetSource().Position.X ?? 0, texture?.GetSource().Position.X ?? 0);
        var size = new Point(region?.Size.X ?? w, region?.Size.Y ?? h);
        Source =  region != null ? new Rectangle(position + region.Value.Position, size) : new Rectangle(new Point(), size);
    }
}