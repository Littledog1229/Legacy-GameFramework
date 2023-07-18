using OpenTK.Mathematics;

namespace Sandbox.Sprite.EditorSprite; 

public interface ILegacyEditorSprite {
    public string EditorTypeName { get; }
    
    public void drawInspectable();
    public void renderPicking(PickingBatch batch, int index);
    public void renderOutline(PickingBatch batch, Vector2 outline_size);
}