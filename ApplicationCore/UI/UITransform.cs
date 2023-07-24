using Engine.Utility;
using OpenTK.Mathematics;

namespace ApplicationCore.UI; 

// TODO: Constrain Min and Max to true position
public class UITransform {
    private readonly List<UITransform>  children    = new();
    private readonly List<UIConstraint> constraints = new();

    private Vector2 position;
    private Vector2 size;
    private Vector2 min;
    private Vector2 max;

    public Vector2 TruePosition => getTruePosition();
    public Vector2 TrueMin      => getTruePosition();
    public Vector2 TrueMax      => getTruePosition() + Size;

    public UIObject     Owner  { get; }
    public UITransform? Parent { get; private set; } = null;

    // All of these (excluding size) are in local space
    public Vector2 Position {
        get => position;
        set {
            if (position == value)
                return;

            position = value;
            min      = position;
            max      = position + size;

            constrain();
        }
    }
    public Vector2 Size     {
        get => size;
        set {
            if (size == value)
                return;

            size = value;
            max  = position + size;
            
            constrain();
            constrainChildren();
        }
    }
    public Vector2 Min      {
        get => min;
        set {
            if (min == value)
                return;

            min = value;
            max = min + size;
            
            constrain();
        }
    }
    public Vector2 Max      {
        get => max;
        set {
            if (max == value)
                return;

            max  = value;
            size = max - min;
            
            constrain();
            constrainChildren();
        }
    }

    public void addConstraint    (UIConstraint constraint) => constraints.Add(constraint);
    public void removeConstraint (UIConstraint constraint) => constraints.Remove(constraint);
    public void clearConstraints ()                        => constraints.Clear();
    
    public void constrain         () {
        foreach (var constraint in constraints)
            constraint.transformed(this);
    }
    public void constrainChildren () {
        foreach (var child in children)
            child.constrainAsChild();
    }

    // Updates the size without calling constraints (useful for constraints)
    public Vector2 RawPosition {
        set {
            if (position == value)
                return;

            position = value;
            min      = position;
            max      = position + size;
        }
    }
    public Vector2 RawSize     {
        set {
            if (size == value)
                return;

            size = value;
            max  = position + size;
        }
    }
    public Vector2 RawMin      {
        set {
            if (min == value)
                return;

            min = value;
            max = min + size;
        }
    }
    public Vector2 RawMax      {
        set {
            if (max == value)
                return;

            max  = value;
            size = max - min;
        }
    }

    public bool isPointInside(Vector2 point) => point.isBetweenOrEqual(TrueMin, TrueMax);

    internal UITransform(UIObject owner) {
        Owner = owner;
    }
    
    private Vector2 getTruePosition() {
        if (Parent == null)
            return position;

        return position + Parent.getTruePosition();
    }
    
    private void constrainAsChild()  {
        foreach (var constraint in constraints)
            constraint.parentResized(this);
    }
    
    internal void addChild    (UITransform child) {
        children.Add(child);
        child.Parent = this;
    }
    internal void removeChild (UITransform child) {
        children.Remove(child);
        if (child.Parent == this)
            child.Parent = null;
    }
}