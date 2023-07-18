using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Texture;
using OpenTK.Mathematics;

namespace Sandbox.Sprite; 

public class SpriteObject {
    protected Vector2 position = Vector2.Zero;
    protected Vector2 scale    = Vector2.One;
    protected float   rotation = 0.0f;

    public string          Identifier { get; set; } = "Sprite";
    public Texture2D?      Texture    { get; set; }
    public Color4          Color      { get; set; } = Color4.White;
    public virtual Vector2 Position {
        get => position;
        set => position = value;
    }
    public virtual Vector2 Scale {
        get => scale;
        set => scale = value;
    }
    public virtual float Rotation {
        get => rotation;
        set => rotation = value;
    }

    public virtual void update()      { }
    public virtual void lateUpdate()  { }
    public virtual void fixedUpdate() { }

    public virtual void render(ShapeBatch batch) {
        batch.drawBox(position, Color, rotation, 0.0f, scale, Texture);
    }

    public virtual void destroy() { }
}