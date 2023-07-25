using OpenTK.Mathematics;

namespace ApplicationCore.UI.Constraint; 

public sealed class PixelConstraint : UIConstraint {
    public ConstraintSide Side   { get; set; }
    public int            Pixels { get; set; }

    public PixelConstraint(ConstraintSide side, int pixels) {
        Side   = side;
        Pixels = pixels;
    } 
    
    public override void parentResized (UITransform transform) => apply(transform);
    public override void transformed   (UITransform transform) => apply(transform);

    private void apply(UITransform transform) {
        if (transform.Parent == null)
            return;
        
        var parent   = transform.Parent!;
        var parent_size = parent.Size;
        var offset      = transform.Position;
        var max         = parent_size - offset;

        transform.RawSize = Side switch {
            ConstraintSide.Bottom => new Vector2(transform.Size.X, max.Y - Pixels),
            ConstraintSide.Right  => new Vector2(max.X - Pixels, transform.Size.Y),
            _                     => transform.Size
        };
    }

    public enum ConstraintSide {
        Bottom,
        Right
    }
}