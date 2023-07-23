using OpenTK.Mathematics;

namespace Sandbox.Serialization; 

public struct SerializableVector2 {
    public float X;
    public float Y;

    public SerializableVector2(Vector2 vector) {
        X = vector.X;
        Y = vector.Y;
    }
}