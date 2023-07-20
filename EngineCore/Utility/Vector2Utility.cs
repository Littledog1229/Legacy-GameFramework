using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace Engine.Utility; 

public static class Vector2Utility {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool greaterThan        (this Vector2 a, Vector2 b) => a.X >  b.X && a.Y >  b.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool greaterThanOrEqual (this Vector2 a, Vector2 b) => a.X >= b.X && a.Y >= b.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool lessThan           (this Vector2 a, Vector2 b) => a.X <  b.X && a.Y <  b.Y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool lessThanOrEqual    (this Vector2 a, Vector2 b) => a.X <= b.X && a.Y <= b.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool isBetweenOrEqual (this Vector2 vector, Vector2 min, Vector2 max) => vector.greaterThanOrEqual(min) && vector.lessThanOrEqual(max);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool isBetween        (this Vector2 vector, Vector2 min, Vector2 max) => vector.greaterThan(min)        && vector.lessThan(max);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2 rotateAround(this Vector2 vector, Vector2 point, float rotation) {
        var local_pos = point - vector;
        var rad = MathHelper.DegreesToRadians(rotation);
        
        var local_rot_pos = new Vector2(
            (local_pos.X * MathF.Cos(rad)) - (local_pos.Y * MathF.Sin(rad)),
            (local_pos.Y * MathF.Cos(rad)) - (local_pos.X * MathF.Sin(rad))
        );

        return point + local_rot_pos;
    }
}