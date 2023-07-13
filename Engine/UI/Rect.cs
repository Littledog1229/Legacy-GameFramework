using Engine.Utility;
using OpenTK.Mathematics;

namespace Engine.UI; 

public struct Rect {
    public float X      { get; set; }
    public float Y      { get; set; }
    public float Width  { get; set; }
    public float Height { get; set; }

    public Rect(Vector2 position, Vector2 size) {
        X      = position.X;
        Y      = position.Y;
        Width  = size.X;
        Height = size.Y;
    }

    public Rect(float x, float y, float width, float height) {
        X      = x;
        Y      = y;
        Width  = width;
        Height = height;
    }
    
    public bool inside(Vector2 mouse_position) {
        var min        = new Vector2(X, Y);
        var max = min + new Vector2(Width, Height);

        return mouse_position.isBetweenOrEqual(min, max);
    }
}