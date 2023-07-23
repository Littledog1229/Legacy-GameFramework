using ApplicationCore.Render.Batch;
using ApplicationCore.Render.Texture;
using OpenTK.Mathematics;
using Sandbox.Serialization;

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

    public SpriteObject() { }
    public SpriteObject(SerializedSprite serialized_sprite) {
        Identifier = (string)  serialized_sprite.Data["Identifier"];
        position   = ((SerializableVector2)serialized_sprite.Data["Position"]).toVector2();
        scale      = ((SerializableVector2)serialized_sprite.Data["Scale"]).toVector2();
        rotation   = (float)   serialized_sprite.Data["Rotation"];
        Color      = (Color4)  serialized_sprite.Data["Color"];
    }
    
    public virtual void update()      { }
    public virtual void lateUpdate()  { }
    public virtual void fixedUpdate() { }

    public virtual void render(ShapeBatch batch) {
        batch.drawBox(position, Color, rotation, 0.0f, scale, Texture);
    }

    public virtual void destroy() { }

    public virtual SerializedSprite serialize() {
        var serialized = new SerializedSprite();

        serialized.Data.Add("Type",       GetType());
        serialized.Data.Add("Identifier", Identifier);
        serialized.Data.Add("Position",   new SerializableVector2(position));
        serialized.Data.Add("Scale",      new SerializableVector2(scale));
        serialized.Data.Add("Rotation",   rotation);
        serialized.Data.Add("Color",      Color);
        
        return serialized;
    }
}