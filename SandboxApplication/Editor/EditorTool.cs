namespace Sandbox.Editor; 

public abstract class EditorTool {
    protected EditorScene   Owner;
    protected EditorSprite? Sprite;

    public EditorTool(EditorScene owner) {
        Owner = owner;
    }
    
    public void setSprite(EditorSprite? sprite) {
        Sprite = sprite;
    }
    public abstract string Identifier { get; }

    // Handle all of the ImGui state
    public    abstract void updateWidget();         // Used to render the imgui widget, as well as handle how the tool itself works
    protected abstract void updateSelectedSprite(); // Used to reset the state when a new sprite is selected
}