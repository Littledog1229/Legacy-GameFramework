using System.Runtime.InteropServices;

namespace Engine.Utility; 

public static class SizeUtility {
    private static readonly Dictionary<Type, int> type_sizes = new();

    public static int getTypeSize<T>() {
        if (type_sizes.TryGetValue(typeof(T), out var size))
            return size;

        var new_size = Marshal.SizeOf<T>();
        type_sizes.Add(typeof(T), new_size);
        return new_size;
    }

    public static int getTypeSize(Type type) {
        if (type_sizes.TryGetValue(type, out var size))
            return size;
        
        var new_size = Marshal.SizeOf(type);
        type_sizes.Add(type, new_size);
        return new_size;
    }
}