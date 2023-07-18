using System.Runtime.CompilerServices;
using OpenTK.Mathematics;
using NativeVector2 = System.Numerics.Vector2;
using PhysicsVector2 = tainicom.Aether.Physics2D.Common.Vector2;

using NativeVector4 = System.Numerics.Vector4;

namespace Engine.Physics; 

public static class VectorExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static NativeVector2  toNativeVector2 (this Vector2        vector) => new(vector.X, vector.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static PhysicsVector2 toPhysicsVector2(this Vector2        vector) => new(vector.X, vector.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2        toVector2       (this PhysicsVector2 vector) => new(vector.X, vector.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector2        toVector2       (this NativeVector2  vector) => new(vector.X, vector.Y);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static NativeVector4 toNativeVector4(this Vector4 vector) => new(vector.X, vector.Y, vector.Z, vector.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static NativeVector4 toNativeVector4(this Color4  vector) => new(vector.R, vector.G, vector.B, vector.A);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 toVector4 (this NativeVector4  vector) => new(vector.X, vector.Y, vector.Z, vector.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector4 toVector4 (this Color4         color)  => new(color.R, color.G, color.B, color.A);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Color4 toColor4  (this Vector4        vector) => new(vector.X, vector.Y, vector.Z, vector.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Color4 toColor4  (this NativeVector4  vector) => new(vector.X, vector.Y, vector.Z, vector.W);
}