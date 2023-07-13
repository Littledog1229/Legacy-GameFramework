using Engine.Application;
using Engine.Render.Batch;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.UI.Element; 

public sealed class TestButtonElement : UIElement {
    public Color4 NormalColor   { get; set; } = Color4.White;
    public Color4 HoveredColor  { get; set; } = Color4.DarkGray;
    public Color4 PressingColor { get; set; } = Color4.Gray;
    
    public TestButtonElement(Vector2 position, Vector2 size) : base(position, size) { }

    public Action<TestButtonElement>? OnPress;

    public override void update() {
        if (!Focused)
            return;

        if (InputManager.mouseDown(MouseButton.Left))
            UIManager.Flags |= UIFlags.FocusLocked;
        else
            UIManager.Flags &= ~UIFlags.FocusLocked;
        
        if (InputManager.mouseRelease(MouseButton.Left))
            OnPress?.Invoke(this);
    }

    public override void render(UIBatch batch) {
        batch.drawRect(BoundingBox, getColor());
    }

    private Color4 getColor() {
        if (!Focused)
            return NormalColor;

        if (InputManager.mouseDown(MouseButton.Left))
            return PressingColor;

        return HoveredColor;
    }
}