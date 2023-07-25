using ApplicationCore.Application;
using Engine.Render.Batch;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ApplicationCore.UI.Elements; 

public sealed class ButtonElement : UIObject {
    public Color4 NormalColor   { get; set; }
    public Color4 HoveredColor  { get; set; }
    public Color4 PressingColor { get; set; }

    public Action? OnPress { get; set; }
    
    private Color4 current_color;

    public ButtonElement() {
        current_color = NormalColor;
    }

    protected internal override void onUpdate() {
        if (!Focused) {
            current_color = NormalColor;
            return;
        }
            

        current_color = HoveredColor;
        
        if (InputManager.mouseDown(MouseButton.Left)) {
            current_color = PressingColor;
            UIManager.Flags |= UIFlags.FocusLocked;
        } else
            UIManager.Flags &= ~UIFlags.FocusLocked;
        
        
        if (InputManager.mouseRelease(MouseButton.Left))
            OnPress?.Invoke();
    }

    protected internal override void onRender(UIBatch batch) {
        batch.drawTransform(Transform, current_color);
    }
}