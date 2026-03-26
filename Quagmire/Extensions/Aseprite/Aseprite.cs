using Quagmire.Assets;
using Quagmire.Rendering;

namespace Quagmire.Extensions.Aseprite;

public class Aseprite : Asset
{
    public struct Tag
    {
        public enum Direction
        {
            Forward = 0,
            Reverse = 1,
            PingPong = 2,
            PingPongReverse = 3,
        }
        
        public string Name;
        public int From;
        public int To;
        public Direction LoopDirection;
        public int Repeat;

        public int this[int frame] => Frame(frame);
        
        private int Frame(int frame)
        {
            if (Repeat > 0)
            {
                if (frame >= Repeat * (To - From + 1))
                {
                    frame = To;
                }
            }
        
            switch (LoopDirection)
            {
                case Direction.Forward:
                    frame = frame % (To - From + 1);
                    break;
                case Direction.Reverse:
                    frame = (From - To + 1) - frame % (From - To + 1);
                    break;
                case Direction.PingPong:
                    var cycleLength = (To - From + 1) * 2 - 2;
                    frame = frame % cycleLength;
                    if (frame >= (To - From + 1))
                    {
                        frame = (To - From) - (frame - (To - From));
                    }
                    frame += From;
                    break;
                case Direction.PingPongReverse:
                    cycleLength = (To - From + 1) * 2 - 2;
                    frame %= cycleLength;
                    if (frame < (To - From + 1))
                    {
                        frame = (To - From) - frame;
                    }
                    else
                    {
                        frame -= (To - From);
                    }
                    frame += From;
                    break;
            }

            return From + frame;
        }
    }
    
    private Dictionary<string, Tag> _tags = [];
    private Dictionary<int,float> _durations = [];
    private TextureAtlas _atlas;
    public TextureAtlas Frames => _atlas;
    public IReadOnlyDictionary<int, float> Durations => _durations;

    public Tag this[string name] => GetTag(name);

    public Aseprite(TextureAtlas atlas, string path) : base(path)
    {
        _atlas = atlas;
    }

    public void CreateTag(string name, int from, int to, Tag.Direction loopDirection = Tag.Direction.Forward, int repeat = 0)
    {
        _tags.Add(name, new Tag
        {
            Name = name,
            From = from,
            To = to,
            LoopDirection = loopDirection,
            Repeat = repeat
        });
    }

    public Tag GetTag(string name)
    {
        if(!_tags.ContainsKey(name))
            throw new Exception("Tag not found");
        return _tags[name];
    }

    public bool HasTag(string name)
    {
        return _tags.ContainsKey(name);
    }
    
    public List<Tag> GetTags()
    {
        return [.._tags.Values];
    }

    public void SetFrameDuration(int frame, float duration)
    {
        _durations[frame] = duration * 0.001f;
    }
}