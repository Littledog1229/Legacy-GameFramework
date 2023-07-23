using OpenTK.Mathematics;

namespace Sandbox3D.Mesh; 

public abstract class MeshGenerator {
    public struct GenerationParameters {
        public Vector3 Offset { get; set; } = Vector3.Zero;
        public Vector3 Scale  { get; set; } = Vector3.One;
        public Vector3 Pivot  { get; set; } = Vector3.Zero;

        public readonly Dictionary<string, object> Parameters = new();
        
        public GenerationParameters() { }
    }
    
    // TODO: Have Transform forms (using matrices)
    public abstract (Vector3[], uint[]) generate(GenerationParameters? parameters = null);
    public abstract void                generate(Vector3[] vertices, uint[] indices, GenerationParameters? parameters = null);
}