using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace ApplicationCore.UI.Elements; 

public sealed class BoxElement : UIObject {
    public Color4 Color { get; set; }

    public BoxElement(Color4 color) {
        Color = color;
    }

    protected internal override void onRender(UIBatch batch) {
        batch.drawTransform(Transform, Color);
    }
}