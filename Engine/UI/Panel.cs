using Engine.Render.Batch;
using OpenTK.Mathematics;

namespace Engine.UI; 

// A special UIObject meant to act as a container for other UIObjects.
// It is primarily meant for representing more advanced objects, such as menus or hud elements
// These are meant to be extended, not really instantiated raw.
public class Panel : UIObject {
    private readonly Dictionary<string, Panel>     children = new();
    private readonly Dictionary<string, UIElement> elements = new();

    private readonly List<UIObject> focusables = new();

    private UIObject? focused_element;
    
    public Panel? Parent { get; internal set; }
    
    public Panel(Vector2 position, Vector2 size) : base(position, size) { }

    // Children Panels
    public void addChild   (string identifier, Panel child) {
        if (child == this)
            return;
        
        if (children.ContainsKey(identifier))
            return;
        
        children.Add(identifier, child);
        child.Parent?.removeChild(identifier);

        child.Parent     = this;
        child.Identifier = identifier;
        //child.FocusabilityChanged += updateFocus;
        
        if (child.Focusable) {
            focusables.Add(child);
            
            // TODO: Update Focus
        }
    }
    public void removeChild(string identifier) {
        if (!children.ContainsKey(identifier))
            return;

        var panel = children[identifier];
        children.Remove(identifier);
        panel.Parent = null;
        //panel.FocusabilityChanged -= updateFocus;
        
        if (!panel.Focusable) 
            return;
        
        focusables.Remove(panel);
        if (focused_element == panel) {
            focused_element = null;
            // TODO: Update focus
        }
    }
    public T    getChild<T>(string identifier) where T : Panel => (T) children[identifier];

    // Children Elements
    public void addElement(string identifier, UIElement element) {
        if (elements.ContainsKey(identifier))
            return;
        
        elements.Add(identifier, element);
        element.Parent?.removeElement(identifier);
        element.Parent     = this;
        element.Identifier = identifier;
        //element.FocusabilityChanged += updateFocus;
        

        if (element.Focusable) {
            focusables.Add(element);
            // TODO: Update Focus
        }
            
    }
    public void removeElement(string identifier) {
        if (!elements.ContainsKey(identifier))
            return;

        var element = elements[identifier];
        elements.Remove(identifier);
        element.Parent = null;
        //element.FocusabilityChanged -= updateFocus;


        if (!element.Focusable)
            return;
        
        focusables.Remove(element);
        if (focused_element == element) {
            focused_element = null;
            // TODO: Update focus
        }
    }
    public T    getElement<T>(string identifier) where T : UIElement => (T) elements[identifier];
    
    protected virtual void onUpdate() { }
    protected virtual void onRender() { }
    
    public sealed override void update() {
        foreach (var (_, element) in elements)
            element.update();
        
        foreach (var (_, panel) in children)
            panel.update();

        onUpdate();
    }
    public sealed override void render(UIBatch batch) {
        foreach (var (_, element) in elements)
            element.render(batch);
        
        foreach (var (_, panel) in children)
            panel.render(batch);
        
        onRender();
    }

    internal void updateFocus(UIObject element, bool value) {
        if (value)
            focusables.Add(element);
        else
            focusables.Remove(element);
    }

    internal UIObject? findFocus(Vector2 mouse_position) {
        if (focusables.Count <= 0) {
            focused_element = null;
            return null;
        }
        
        UIObject? grab_focus = null;

        foreach (var focusable in focusables.Where(focusable => focusable.canFocus(mouse_position))) {
            grab_focus ??= focusable;

            if (focusable.FocusPriority > grab_focus.FocusPriority)
                grab_focus = focusable;
        }

        focused_element = grab_focus;
        if (focused_element is Panel panel)
            return panel.findFocus(mouse_position) ?? panel;

        return grab_focus;
    }
}