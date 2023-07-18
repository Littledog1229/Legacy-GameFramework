using ApplicationCore.Render.Batch;
using OpenTK.Mathematics;
using Sandbox.Sprite;

namespace Sandbox.Editor; 

public sealed class EditorSprite {
    public EditorScene  Owner      { get; private set; }
    public Type         SpriteType { get; private set; }
    public SpriteObject Sprite     { get; private set; }

    public EditorSprite(EditorScene owner, SpriteObject sprite) {
        Owner      = owner;
        SpriteType = sprite.GetType();
        Sprite     = sprite;
    }
    
    // Sprite methods
    public void update()                 => Sprite.update();
    public void lateUpdate()             => Sprite.lateUpdate();
    public void fixedUpdate()            => Sprite.fixedUpdate();
    public void render(ShapeBatch batch) => Sprite.render(batch);
    public void destroy()                => Sprite.destroy();
    
    // Editor Specific Things
    // TODO: Make more versatile?
    public void renderPicking(PickingBatch batch, int index)            => batch.drawBox((uint) index, Sprite.Position, Sprite.Rotation, 0.0f, Sprite.Scale);
    public void renderOutline(PickingBatch batch, Vector2 outline_size) => batch.drawBox(0u, Sprite.Position, Sprite.Rotation, 0.0f, Sprite.Scale + outline_size);
}