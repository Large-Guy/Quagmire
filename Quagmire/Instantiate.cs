namespace Quagmire;

public static class Instantiate
{
    private static Dictionary<string, Type> _types = [];

    public static object New(string typeName)
    {
        if (!_types.TryGetValue(typeName, out var type))
        {
            type = Type.GetType(typeName) 
                       ?? AppDomain.CurrentDomain.GetAssemblies()
                           .Select(a => a.GetType(typeName))
                           .FirstOrDefault(t => t != null)
                       ?? throw new Exception($"Unable to find Asset Loader: {typeName}");
                
            _types[typeName] = type;
        }
        
        var obj = Activator.CreateInstance(type) ?? throw new Exception($"Unable to create {type}");
        return obj;
    }

    public static T New<T>(string typeName) where T : class
    {
        return New(typeName) as T ?? throw new Exception($"{typeName} is not {typeof(T)}");
    }
}