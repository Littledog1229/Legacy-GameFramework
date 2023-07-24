using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace ApplicationCore.UI; 

// TODO: Mark focus as dirty when focused and focusability changed
// TODO: Store focused child and mark focus dirty when its (or its parent's) focusability changes
public abstract class UIObject {
    private bool focusable              = false;
    private bool children_focusable     = false;
    private bool has_focusable_children = false;

    private          string                       identifier = "";
    private          UIObject?                    parent     = null;
    private readonly Dictionary<string, UIObject> children   = new();
    
    // Will contain any element that is focusable, or has focusable children
    private readonly List<UIObject> focusables = new();

    public string      Identifier       {
        get => identifier;
        set {
            if (identifier == value)
                return;

            parent?.updateChildIdentifier(this, identifier, value);
            identifier = value;
        }
    }
    public bool        Focusable        {
        get => focusable;
        set {
            if (focusable == value)
                return;

            focusable = value;
            
            if (value == false && children_focusable == false)
                parent?.removeFocusable(this);
            else if (children_focusable == false)
                parent?.addFocusable(this);

            UIManager.Flags |= UIFlags.FocusDirty;
        }
    }
    public bool        CanFocusChildren {
        get => children_focusable;
        set {
            if (children_focusable == value)
                return;

            children_focusable = value;
            
            if (!value && has_focusable_children && !focusable)
                parent?.removeFocusable(this);
            else if (!focusable && has_focusable_children)
                parent?.addFocusable(this);
            
            UIManager.Flags |= UIFlags.FocusDirty;
        }
    }
    public UIObject?   Parent           {
        get => parent;
        set {
            if (parent == value)
                return;
            
            setParent(value);
        }
    }
    public bool        Focused          { get; internal set; }
    public int         FocusPriority    { get; set; }
    public UITransform Transform        { get; }

    protected UIObject() { Transform = new(this); }
    
    public void      addChild    (UIObject child)          => child.setParent(this);
    public void      removeChild (UIObject child)          => child.setParent(null); // Could technically be static, but it feels weird to do so
    public UIObject? getChild    (string child_identifier) => children!.GetValueOrDefault(child_identifier, null);

    public void resize(int width, int height) => Transform.Size = new Vector2(width, height);
    
    protected internal virtual void onFocusGained() { }
    protected internal virtual void onFocusLost()   { }
    
    protected internal virtual void onUpdate() { }
    protected internal virtual void onRender(UIBatch batch) { }

    internal bool hasFocusability ()                 => focusable || (children_focusable && has_focusable_children);
    internal bool canFocus        (Vector2 position) => hasFocusability() && Transform.isPointInside(position);

    internal void update() {
        onUpdate();

        foreach (var (_, child) in children)
            child.update();
    }
    internal void render(UIBatch batch) {
        onRender(batch);

        foreach (var (_, child) in children)
            child.render(batch);
    }
    
    // Children that can be focused always take precedence over its parent
    internal UIObject? findFocus(Vector2 position) {
        if (focusables.Count <= 0)
            return canFocus(position) ? this : null;

        UIObject? grab_focus = null;

        foreach (var focusable_object in focusables.Where(focusable_object => focusable_object.canFocus(position))) {
            grab_focus ??= focusable_object.findFocus(position);

            if (focusable_object.FocusPriority > grab_focus?.FocusPriority)
                grab_focus = focusable_object.findFocus(position);
        }

        grab_focus = grab_focus?.findFocus(position);

        if (grab_focus == null && focusable)
            return this;

        return grab_focus;
    }
    
    private void removeFocusable (UIObject focusable_object) {
        focusables.Remove(focusable_object);
        if (focusables.Count <= 0 && (children_focusable || focusable))
            parent?.removeFocusable(this);

        has_focusable_children = focusables.Count > 0;
    }
    private void addFocusable    (UIObject focusable_object) {
        focusables.Add(focusable_object);
        if (focusables.Count == 1 && (children_focusable || focusable))
            parent?.addFocusable(this);
        
        has_focusable_children = focusables.Count > 0;
    }
    
    private void updateChildIdentifier(UIObject child, string old_identifier, string new_identifier) {
        children.Remove(old_identifier);

#if DEBUG
        if (children.ContainsKey(new_identifier))
            throw new Exception($"UIObject {this} already contains child with identifier: {new_identifier}");
#endif
        
        children.Add(new_identifier, child);
    }
    private void setParent(UIObject? new_parent) {
        parent?.removeChildInternal(this);
        parent = new_parent;
        parent?.addChildInternal(this);
        
        UIManager.Flags |= UIFlags.FocusDirty;
    }

    private void addChildInternal(UIObject child) {
        children.Add(child.identifier, child);
        Transform.addChild(child.Transform);
        
        if (child.hasFocusability())
            addFocusable(child);
    }
    private void removeChildInternal(UIObject child) {
        children.Remove(child.identifier);
        Transform.removeChild(child.Transform);

        if (child.hasFocusability())
            removeFocusable(child);
    }
}