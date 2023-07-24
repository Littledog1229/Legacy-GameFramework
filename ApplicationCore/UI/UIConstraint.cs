namespace ApplicationCore.UI; 

// Typically only used for updating an objects size
public abstract class UIConstraint {
    public abstract void parentResized (UITransform transform);
    public abstract void transformed   (UITransform transform);
}