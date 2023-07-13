using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Engine.Render.Camera; 

public sealed class OrthoCamera : Camera {
    private Vector2 position    = Vector2.Zero;
    private float   view_size_x = 5.0f;

    public OrthoCamera() { }
    
    public OrthoCamera(Vector2 position, float view_size_x = 5.0f) {
        this.position    = position;
        this.view_size_x = view_size_x;
    }
    
    public Vector2 Position {
        get => position;
        set {
            if (value == position)
                return;

            position = value;
            createView();
        }
    }

    public float ViewSizeX {
        get => view_size_x;
        set {
            if (Math.Abs(value - view_size_x) < 0.01f)
                return;

            view_size_x = value;
            createProjection();
        }
    }

    protected override void createView() {
        View = Matrix4.LookAt(new Vector3(position.X, position.Y, 10.0f), new Vector3(position.X, position.Y, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
    }

    protected override void createProjection() {
        var aspect_ratio = (float) RenderManager.WindowHeight / (float) RenderManager.WindowWidth;
        var width  = view_size_x * 2.0f;
        var height = width * aspect_ratio;
        
        Projection = Matrix4.CreateOrthographic(width, height, 0.1f, 100.0f);
    }
}