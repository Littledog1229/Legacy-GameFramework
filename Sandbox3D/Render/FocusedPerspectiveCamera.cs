using ApplicationCore.Render.Camera;
using OpenTK.Mathematics;

namespace Sandbox3D.Render; 

public sealed class FocusedPerspectiveCamera : Camera {
    public static readonly Vector3 UP = new(0.0f, 1.0f, 0.0f);
    
    private Vector3 position;
    private Vector3 focus;

    public Vector3 Position {
        get => position;
        set {
            if (position == value)
                return;

            position = value;
            createView();
        }
    }
    public Vector3 Focus {
        get => focus;
        set {
            if (focus == value)
                return;

            focus = value;
            createView();
        }
    }

    public FocusedPerspectiveCamera(Vector3? position = null, Vector3? focus = null) {
        this.position = position ?? Vector3.Zero;
        this.focus    = focus    ?? Vector3.Zero;
    }
    
    protected override void createView() {
        View = Matrix4.LookAt(Position, Focus, Vector3.UnitY);
    }

    protected override void createProjection() {
        Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75.0f), (float)Width / (float)Height, 0.1f, 100.0f);
    }
}