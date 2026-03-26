using System.IO.Compression;
using System.Numerics;
using Quagmire.Assets;
using Quagmire.Rendering;
using SDL3;

namespace Quagmire.Extensions.Aseprite;

public class AsepriteLoader : IAssetLoader
{
    private FileStream _file;
    private BinaryReader _reader;
    private Header _ase;
    
    public struct Header
    {
        //Header data
        public uint FileSize;
        public ushort MagicNumber;

        public ushort FramesNumber;
        public ushort Width;
        public ushort Height;
        public ushort ColorDepth;
        public uint Opacity;
        public ushort Speed;

        public byte PaletteEntry;

        public short NumberColor;
        public byte PixelWidth;
        public byte PixelHeight;
        public short GridX;
        public short GridY;
        public ushort GridWidth;
        public ushort GridHeight;

        public FrameHeader[] Frames;
    }

    public struct FrameHeader
    {
        public uint BytesSize;
        public ushort MagicNumber;
        
        public ushort FrameDuration;
        
        public long ChunksNumber;

        public Chunk[] Chunks;
    }

    public struct ColorProfile
    {
        public short type;
        public short uses_fixed_gama;
        public int fixed_game;
    }

    public struct Palette
    {
        public uint EntrySize;
        public uint FirstColor;
        public uint LastColor;
        public uint[] colors;
    }

    public struct OldPalette
    {
        public ushort Packets;
        public uint[][] ColorPackets;
    }

    public struct Layer
    {
        public ushort Flags;
        public ushort Type;
        public ushort ChildLevel;
        public ushort Width;
        public ushort Height;
        public ushort Blend;
        public byte Opacity;
        public string Name;
    }

    public struct Cel
    {
        public ushort LayerIndex;
        public ushort X;
        public ushort Y;
        public byte OpacityLevel;
        public ushort Type;
        public ushort Width;
        public ushort Height;
        public byte[] Data;
        
        //Linked Cell
        public ushort FramePosLink;
        
        //Compressed Tilemap
        public ushort BitsPerTile;
        public uint BitmaskTileID;
        public uint BitmaskXFlip;
        public uint BitmaskYFlip;
        public uint BitmaskRotation;
    }

    public struct Tags
    {
        public struct Tag
        {
            public ushort From;
            public ushort To;
            public byte Direction;
            public byte Repeat;
            public byte R;
            public byte G;
            public byte B;
            public UInt64 SkipHolder;
            public string Name;
        }
        
        public ushort Number;
        public Tag[] Data;
    }

    public struct Slice
    {
        public struct Key
        {
            public uint Frame;
            public uint X;
            public uint Y;
            public uint Width;
            public uint Height;
            public uint CenterX;
            public uint CenterY;
            public uint CenterWidth;
            public uint CenterHeight;
            public uint PivotX;
            public uint PivotY;
        }
        
        public uint KeyNumbers;
        public Key[] Keys;
        public uint Flags;
        public string Name;
    }

    public struct UserData
    {
        public uint Flags;
        public string Text;
        public uint Colors;
    }

    public struct TileSet
    {
        public uint Id;
        public uint Flags;
        public uint NumTiles;
        public ushort TileWidth;
        public ushort TileHeight;
        public short BaseIndex;
        public string Name;
        public uint ExternalId;
        public uint TilesetIdInExternalFile;
    }

    public struct Chunk
    {
        public uint Size;
        public ushort Type;
        public object Data;
    }

    public enum FormatType
    {
        Byte,   // unsigned 8-bit
        Word,   // unsigned 16-bit
        Short,  // signed 16-bit
        DWord,  // unsigned 32-bit
        Long,   // signed 32-bit
        Fixed   // signed 32-bit (usually interpreted as 16.16 fixed-point)
    }
    
    public int ReadNum(FormatType format)
    {
        // You can modify this to return an array if `amount > 1` is needed.
        switch (format)
        {
            case FormatType.Byte:
                return _reader.ReadByte();

            case FormatType.Word:
                return _reader.ReadUInt16();

            case FormatType.Short:
                return _reader.ReadInt16();

            case FormatType.DWord:
                return (int)_reader.ReadUInt32(); // or use uint if you prefer

            case FormatType.Long:
            case FormatType.Fixed:
                return _reader.ReadInt32();

            default:
                throw new ArgumentException("Unknown format type");
        }
    }

    public void Skip(int amount)
    {
        _reader.BaseStream.Seek(amount, SeekOrigin.Current);
    }

    private string ReadString()
    {
        short length = (short)ReadNum(FormatType.Short);
        byte[] chars = new byte[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = (byte)ReadNum(FormatType.Byte);
        }
        return System.Text.Encoding.UTF8.GetString(chars);
    }

    public AsepriteLoader()
    {
        
    }

    private Header LoadHeader()
    {
        var header = new Header();
        header.FileSize = (uint)ReadNum(FormatType.DWord);
        header.MagicNumber = (ushort)ReadNum(FormatType.Word);

        if (header.MagicNumber != 0xA5E0)
        {
            throw new Exception("Invalid Aseprite file");
        }
        
        header.FramesNumber = (ushort)ReadNum(FormatType.Word);
        header.Width = (ushort)ReadNum(FormatType.Word);
        header.Height = (ushort)ReadNum(FormatType.Word);
        header.ColorDepth = (ushort)ReadNum(FormatType.Word);
        header.Opacity = (uint)ReadNum(FormatType.DWord);
        header.Speed = (ushort)ReadNum(FormatType.Word);
        
        Skip(8);
        header.PaletteEntry = (byte)ReadNum(FormatType.Byte);
        
        Skip(3);
        header.NumberColor = (short)ReadNum(FormatType.Short);
        header.PixelWidth = (byte)ReadNum(FormatType.Byte);
        header.PixelHeight = (byte)ReadNum(FormatType.Byte);
        header.GridX = (short)ReadNum(FormatType.Short);
        header.GridY = (short)ReadNum(FormatType.Short);
        header.GridWidth = (ushort)ReadNum(FormatType.Word);
        header.GridHeight = (ushort)ReadNum(FormatType.Word);
        
        Skip(84);
        
        header.Frames = new FrameHeader[header.FramesNumber];

        return header;
    }

    private FrameHeader LoadFrameHeader()
    {
        var header = new FrameHeader();
        header.BytesSize = (uint)ReadNum(FormatType.DWord);
        header.MagicNumber = (ushort)ReadNum(FormatType.Word);

        if (header.MagicNumber != 0xF1FA)
        {
            throw new Exception("Invalid Aseprite file");
        }
        
        ushort oldChunks = (ushort)ReadNum(FormatType.Word);
        
        header.FrameDuration = (ushort)ReadNum(FormatType.Word);

        Skip(2);
        
        long newChunks = (long)ReadNum(FormatType.DWord);

        if (newChunks == 0)
        {
            header.ChunksNumber = oldChunks;
        }
        else
        {
            header.ChunksNumber = newChunks;
        }
        
        header.Chunks = new Chunk[header.ChunksNumber];
        
        return header;
    }
    
    private ColorProfile GrabColorProfile()
    {
        var profile = new ColorProfile();
        profile.type = (short)ReadNum(FormatType.Word);
        profile.uses_fixed_gama = (short)ReadNum(FormatType.Word);
        profile.fixed_game = ReadNum(FormatType.Fixed);
        Skip(8);
        if (profile.type == 0)
        {
            Console.WriteLine("No supported color profile, using sRGB");
        }

        if (profile.type == 2)
        {
            var iccProfileLength = (uint)ReadNum(FormatType.DWord);
            //Hopefully we can just skip
            Skip((int)iccProfileLength);
        }
        return profile;
    }

    private Palette GrabPalette()
    {
        var palette = new Palette();
        palette.EntrySize = (uint)ReadNum(FormatType.DWord);
        palette.FirstColor = (uint)ReadNum(FormatType.DWord);
        palette.LastColor = (uint)ReadNum(FormatType.DWord);
        palette.colors = new uint[palette.EntrySize];
        
        Skip(8);
        
        for (int i = 0; i < palette.EntrySize; i++)
        {
            var has_name = (ushort)ReadNum(FormatType.Word);
            palette.colors[i] = (uint)ReadNum(FormatType.DWord);
            if (has_name == 1)
            {
                ReadString(); //We don't need this
            }
        }
        return palette;
    }

    private OldPalette GrabOldPalette()
    {
        var palette = new OldPalette();
        palette.Packets = (ushort)ReadNum(FormatType.Word);
        palette.ColorPackets = new uint[palette.Packets][];

        for (int i = 0; i < palette.Packets; i++)
        {
            var entries = (byte)ReadNum(FormatType.Byte);
            var number = (short)(byte)ReadNum(FormatType.Byte);

            if (number == 0)
            {
                number = 256;
            }
            
            palette.ColorPackets[i] = new uint[number];

            for (int j = 0; j < number; j++)
            {
                var r = (byte)ReadNum(FormatType.Byte);
                var g = (byte)ReadNum(FormatType.Byte);
                var b = (byte)ReadNum(FormatType.Byte);
                
                uint color = (uint)((r << 16) | (g << 8) | b);
                //Also put full alpha on there
                color |= 0xFF000000;
                palette.ColorPackets[i][j] = color;
            }
        }
        
        return palette;
    }

    private Layer GrabLayer()
    {
        var layer = new Layer();
        layer.Flags = (ushort)ReadNum(FormatType.Word);
        layer.Type = (ushort)ReadNum(FormatType.Word);
        layer.ChildLevel = (ushort)ReadNum(FormatType.Word);
        layer.Width = (ushort)ReadNum(FormatType.Word);
        layer.Height = (ushort)ReadNum(FormatType.Word);
        layer.Blend = (ushort)ReadNum(FormatType.Word);
        layer.Opacity = (byte)ReadNum(FormatType.Byte);
        Skip(3);
        layer.Name = ReadString();
        return layer;
    }

    private Cel GrabCel(uint size)
    {
        var cel = new Cel();
        
        cel.LayerIndex = (ushort)ReadNum(FormatType.Word);
        cel.X = (ushort)ReadNum(FormatType.Word);
        cel.Y = (ushort)ReadNum(FormatType.Word);
        cel.OpacityLevel = (byte)ReadNum(FormatType.Byte);
        cel.Type = (ushort)ReadNum(FormatType.Word);

        Skip(7);
        
        if (cel.Type == 0) //Raw image
        {
            cel.Width = (ushort)ReadNum(FormatType.Word);
            cel.Height = (ushort)ReadNum(FormatType.Word);
            cel.Data = new byte[cel.Width * cel.Height * 4];

            for (int i = 0; i < cel.Width * cel.Height * 4; i++)
            {
                var v = (byte)ReadNum(FormatType.Byte);
                cel.Data[i] = v;
            }
        }
        else if(cel.Type == 1) //Linked cell
        {
            cel.FramePosLink = (ushort)ReadNum(FormatType.Word);
        }
        else if (cel.Type == 2) //Compressed image
        {
            cel.Width = (ushort)ReadNum(FormatType.Word);
            cel.Height = (ushort)ReadNum(FormatType.Word);
            cel.Data = new byte[size - 26];
            for (int i = 0; i < size - 26; i++)
            {
                var v = (byte)ReadNum(FormatType.Byte);
                cel.Data[i] = v;
            }
        }
        else if (cel.Type == 3) //Compressed tilemap
        {
            cel.Width = (ushort)ReadNum(FormatType.Word);
            cel.Height = (ushort)ReadNum(FormatType.Word);
            cel.BitsPerTile = (ushort)ReadNum(FormatType.Word);
            cel.BitmaskTileID = (uint)ReadNum(FormatType.DWord);
            cel.BitmaskXFlip = (uint)ReadNum(FormatType.DWord);
            cel.BitmaskYFlip = (uint)ReadNum(FormatType.DWord);
            cel.BitmaskRotation = (uint)ReadNum(FormatType.DWord);
            
            Skip(10);
            
            cel.Data = new byte[cel.Width * cel.Height * 4];
            for (int i = 0; i < cel.Width * cel.Height * 4; i++)
            {
                var v = (byte)ReadNum(FormatType.Byte);
                cel.Data[i] = v;
            }
        }

        return cel;
    }

    private Tags GrabTags()
    {
        var tags = new Tags();
        tags.Number = (ushort)ReadNum(FormatType.Word);
        tags.Data = new Tags.Tag[tags.Number];
        
        Skip(8);

        for (int i = 0; i < tags.Number; i++)
        {
            tags.Data[i] = new Tags.Tag();
            tags.Data[i].From = (ushort)ReadNum(FormatType.Word);
            tags.Data[i].To = (ushort)ReadNum(FormatType.Word);
            tags.Data[i].Direction = (byte)ReadNum(FormatType.Byte);
            tags.Data[i].Repeat = (byte)ReadNum(FormatType.Byte);
            tags.Data[i].R = (byte)ReadNum(FormatType.Byte);
            tags.Data[i].G = (byte)ReadNum(FormatType.Byte);
            tags.Data[i].B = (byte)ReadNum(FormatType.Byte);
            tags.Data[i].SkipHolder = (UInt64)(ReadNum(FormatType.DWord) | (ReadNum(FormatType.DWord) << 32));
            tags.Data[i].Name = ReadString();
        }

        return tags;
    }

    private Slice GrabSlice()
    {
        var slice = new Slice();
        slice.KeyNumbers = (uint)ReadNum(FormatType.DWord);
        slice.Keys = new Slice.Key[slice.KeyNumbers];
        slice.Flags = (uint)ReadNum(FormatType.DWord);
        Skip(4);
        slice.Name = ReadString();

        for (int i = 0; i < slice.KeyNumbers; i++)
        {
            slice.Keys[i] = new Slice.Key();
            slice.Keys[i].Frame = (uint)ReadNum(FormatType.DWord);
            slice.Keys[i].X = (uint)ReadNum(FormatType.DWord);
            slice.Keys[i].Y = (uint)ReadNum(FormatType.DWord);
            slice.Keys[i].Width = (uint)ReadNum(FormatType.DWord);
            slice.Keys[i].Height = (uint)ReadNum(FormatType.DWord);

            if (slice.Flags == 1)
            {
                slice.Keys[i].CenterX = (uint)ReadNum(FormatType.DWord);
                slice.Keys[i].CenterY = (uint)ReadNum(FormatType.DWord);
                slice.Keys[i].CenterWidth = (uint)ReadNum(FormatType.DWord);
                slice.Keys[i].CenterHeight = (uint)ReadNum(FormatType.DWord);
            }
            else if (slice.Flags == 2)
            {
                slice.Keys[i].PivotX = (uint)ReadNum(FormatType.DWord);
                slice.Keys[i].PivotY = (uint)ReadNum(FormatType.DWord);
            }
        }

        return slice;
    }

    private UserData GrabUserData()
    {
        var userData = new UserData();
        userData.Flags = (uint)ReadNum(FormatType.DWord);

        if (userData.Flags == 1)
        {
            userData.Text = ReadString();
        }
        else if (userData.Flags == 2)
        {
            userData.Colors = (uint)ReadNum(FormatType.DWord);
        }

        return userData;
    }

    private TileSet GrabTileSet()
    {
        var tileSet = new TileSet();
        
        tileSet.Id = (uint)ReadNum(FormatType.DWord);
        tileSet.Flags = (uint)ReadNum(FormatType.DWord);
        tileSet.NumTiles = (uint)ReadNum(FormatType.DWord);
        tileSet.TileWidth = (ushort)ReadNum(FormatType.Word);
        tileSet.TileHeight = (ushort)ReadNum(FormatType.Word);
        tileSet.BaseIndex = (short)ReadNum(FormatType.Short);
        
        Skip(14);
        
        tileSet.Name = ReadString();

        if (tileSet.Flags == 1)
        {
            tileSet.ExternalId = (uint)ReadNum(FormatType.DWord);
            tileSet.TilesetIdInExternalFile = (uint)ReadNum(FormatType.DWord);
        }
        else if (tileSet.Flags == 2)
        {
            throw new Exception("Compressed tileset unsupported");
        }

        return tileSet;
    }

    private Chunk GrabChunk()
    {
        var chunk = new Chunk();
        chunk.Size = (uint)ReadNum(FormatType.DWord);
        chunk.Type = (ushort)ReadNum(FormatType.Word);
        chunk.Data = null;

        if (chunk.Type == 0x2007)
        {
            chunk.Data = GrabColorProfile();
        }
        else if (chunk.Type == 0x2019)
        {
            chunk.Data = GrabPalette();
        }
        else if (chunk.Type == 0x0004)
        {
            chunk.Data = GrabOldPalette();
        }
        else if (chunk.Type == 0x2004)
        {
            chunk.Data = GrabLayer();
        }
        else if (chunk.Type == 0x2005)
        {
            chunk.Data = GrabCel(chunk.Size);
        }
        else if (chunk.Type == 0x2018)
        {
            chunk.Data = GrabTags();
        }
        else if (chunk.Type == 0x2022)
        {
            chunk.Data = GrabSlice();
        }
        else if (chunk.Type == 0x2020)
        {
            chunk.Data = GrabUserData();
        }
        else if (chunk.Type == 0x2023)
        {
            chunk.Data = GrabTileSet();
        }

        return chunk;
    }

    public Asset Load(AssetPack pack, string path)
    {
        _file = File.OpenRead(path);
        _reader = new BinaryReader(_file);
        var header = LoadHeader();

        var atlas = new TextureAtlas(path);
        var aseprite = new Aseprite(atlas, path);

        for (var i = 0; i < header.FramesNumber; i++)
        {
            header.Frames[i] = LoadFrameHeader();
            
            for (var j = 0; j < header.Frames[i].ChunksNumber; j++)
            {
                header.Frames[i].Chunks[j] = GrabChunk();
            }
        }

        _ase = header;
        
        _file.Close();
        _reader.Close();
        
        var layers = new List<Layer>();

        for (var index = 0; index < _ase.Frames.Length; index++)
        {
            var frame = _ase.Frames[index];
            foreach (var chunk in frame.Chunks)
            {
                if (chunk is { Type: 0x2005, Data: Cel cel })
                {
                    var texture = LoadCelData(cel, layers);
                        
                    atlas.Queue(texture);
                    aseprite.SetFrameDuration(index, frame.FrameDuration);
                }

                if (chunk is { Type: 0x2018, Data: Tags tags })
                {
                    foreach (var tag in tags.Data)
                    {
                        aseprite.CreateTag(tag.Name, tag.From, tag.To, (Aseprite.Tag.Direction)tag.Direction, tag.Repeat);
                    }
                }

                if (chunk is { Type: 0x2004, Data: Layer layer })
                {
                    layers.Add(layer);
                }
            }
        }
        
        atlas.Build();

        return aseprite;
    }

    private unsafe Texture2D LoadCelData(Cel cel, List<Layer> layers)
    {
        byte[] data = cel.Data;
        switch (cel.Type)
        {
            case 1:
                throw new Exception("Linked cell not supported quite yet");
            case 2:
            {
                using var inputStream = new MemoryStream(data);
                using var zlibStream = new ZLibStream(inputStream, CompressionMode.Decompress);
                using var outputStream = new MemoryStream();
                zlibStream.CopyTo(outputStream);

                data = outputStream.ToArray();
                break;
            }
        }

        var canvas = new byte[_ase.Width * _ase.Height * 4];

        fixed (byte* p = data, c = canvas)
        {
            var pData = (uint*)p;
            var pCanvas = (uint*)c;
            for (var y = 0; y < cel.Height; y++)
            {
                for (var x = 0; x < cel.Width; x++)
                {
                    var px = x + cel.X;
                    var py = y + cel.Y;

                    //Extract the alpha channel
                    var a = (byte)(pData[x + y * cel.Width] >> 24);

                    a = (byte)(a * ((float)cel.OpacityLevel / 255f) *
                               (layers[cel.LayerIndex].Opacity / 255f));

                    //Put it back in
                    pData[x + y * cel.Width] =
                        (uint)(a << 24) | (pData[x + y * cel.Width] & 0x00FFFFFF);

                    pCanvas[px + py * _ase.Width] = pData[x + y * cel.Width];
                }
            }
        }

        var texture = new Texture2D(_ase.Width, _ase.Height, canvas);
        return texture;
    }
}