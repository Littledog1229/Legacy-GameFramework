using System.Reflection;

namespace Engine.Utility; 

public static class FileUtility {
    public static string readEmbeddedRaw(string embedded_path, Assembly? assembly = null) {
        if (assembly == null) 
            assembly = EngineCore.ApplicationAssembly;
        string resource_name;
        
        try { resource_name = assembly.GetManifestResourceNames().Single(str => str.EndsWith(embedded_path)); }
        catch (Exception) {
            Console.WriteLine($"Could not get resource: {embedded_path}");
            return "";
        }

        using var stream = assembly.GetManifestResourceStream(resource_name);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
    
    // MUST manage the lifetime of the returned stream
    public static Stream? getEmbeddedStream(string embedded_path, Assembly? assembly = null) {
        if (assembly == null) 
            assembly = EngineCore.ApplicationAssembly;
        
        string resource_name;
        
        try { resource_name = assembly.GetManifestResourceNames().Single(str => str.EndsWith(embedded_path)); }
        catch (Exception) {
            Console.WriteLine($"Could not get resource: {embedded_path}");
            return null;
        }

        return assembly.GetManifestResourceStream(resource_name);
    }
}