using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace Engine.UI; 

public abstract class UIObject {
    private bool focusable          = false;
    private bool can_focus_children = true;
    private bool focused            = false;

    private UIObject?                             parent   = null;
    private readonly Dictionary<string, UIObject> children = new();

    private string identifier = "";

    // Anything that can be focused, or has children that can be focused (with can_focus_children enabled)
    private readonly List<UIObject> has_focusability = new();

    public string Identifier {
        get => identifier;
        set {
            if (identifier == value)
                return;

            identifier = value;
            // TODO: Update identifier in parent
        }
    }
    public bool   CanFocusChildren {
        get => can_focus_children;
        set {
            if (can_focus_children == value)
                return;

            can_focus_children = value;
            // TODO: Update Child Focusable
        }
    }
    public bool   Focusable        {
        get => focusable;
        set {
            if (focusable == value)
                return;

            focusable = value;
            // TODO: Swap from Focusability callback to calling parent's set focusability of an object
            //FocusabilityChanged?.Invoke(this, value);
        }
    }
    public int    FocusPriority    { get; set; }
    public bool   Focused          { get; internal set; }
    public Rect   BoundingBox      { get; protected set; }

    //public Action<UIObject, bool>? FocusabilityChanged { get; set; }

    public UIObject(Vector2 position, Vector2 size) {
        BoundingBox = new Rect(position, size);
    }

    public void reposition (float x,     float y)      => BoundingBox = new Rect(x, y, BoundingBox.Width, BoundingBox.Height);
    public void resize     (float width, float height) => BoundingBox = new Rect(BoundingBox.X, BoundingBox.Y, width, height);

    public virtual void focusGained() { }
    public virtual void focusLost()   { }
    
    public virtual void update() { }
    public virtual void render(UIBatch batch) { }
    
    public bool canFocus(Vector2 mouse_position) => focusable && BoundingBox.inside(mouse_position);
}