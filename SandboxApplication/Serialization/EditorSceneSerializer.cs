using Newtonsoft.Json;

namespace Sandbox.Serialization; 

public sealed class EditorSceneSerializer {
    public void serialize(EditorScene scene) {
        var serializer = new JsonSerializer();
    }
}