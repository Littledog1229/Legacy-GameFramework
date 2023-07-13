using OpenTK.Mathematics;

namespace Engine.UI; 

public class UIElement : UIObject {
    public Panel? Parent     { get; internal set; }
    
    public UIElement(Vector2 position, Vector2 size) : base(position, size) { }
}