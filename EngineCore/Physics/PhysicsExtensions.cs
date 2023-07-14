using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace Engine.Physics; 

public static class PhysicsExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2                                  toVector2(this tainicom.Aether.Physics2D.Common.Vector2 vector) => new(vector.X, vector.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static tainicom.Aether.Physics2D.Common.Vector2 toVector2(this Vector2 vector)                                  => new(vector.X, vector.Y);
}