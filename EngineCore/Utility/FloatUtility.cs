using OpenTK.Mathematics;

namespace Engine.Utility; 

public static class FloatUtility {
    public static float toRadians(this float value) => MathHelper.DegreesToRadians(value);
    public static float toDegrees(this float value) => MathHelper.RadiansToDegrees(value);
}