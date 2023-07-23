using OpenTK.Mathematics;
using Sandbox.Serialization;

namespace Sandbox; 

public static class Extensions {
    public static Vector2 toVector2(this SerializableVector2 vector) => new(vector.X, vector.Y); 
    
}