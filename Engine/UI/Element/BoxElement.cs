using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace Engine.UI.Element; 

public class BoxElement : UIElement {
    public BoxElement(Vector2 position, Vector2 size) : base(position, size) { }

    public override void render(UIBatch batch) {
        batch.drawRect(BoundingBox, Color4.White);
    }

    public override void focusGained() {
        Console.WriteLine("FOCUSED");
    }
}