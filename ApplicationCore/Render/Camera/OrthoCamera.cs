using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace ApplicationCore.Render.Camera; 

public sealed class OrthoCamera : Camera {
    private Vector2 position    = Vector2.Zero;
    private float   view_size_x = 5.0f;
    private float   view_size_y = 0.0f;

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

    public Vector2 screenToWorldSpace(Vector2 screen_point) {
        var ratio_x = screen_point.X / Width;
        var ratio_y = screen_point.Y / Height;
        
        var normalized_x = ratio_x * 2.0f - 1.0f;
        var normalized_y = ratio_y * 2.0f - 1.0f;

        var relative_world_x =  view_size_x * normalized_x;
        var relative_world_y = -view_size_y * normalized_y;

        return position + new Vector2(relative_world_x, relative_world_y);
    }

    public Vector2 worldToScreenSpace(Vector2 world_point) {
        var normalized_x = (world_point.X - position.X) /  view_size_x;
        var normalized_y = (world_point.Y - position.Y) / -view_size_y;

        var ratio_x = (normalized_x + 1.0f) / 2.0f;
        var ratio_y = (normalized_y + 1.0f) / 2.0f;

        var screen_x = ratio_x * Width;
        var screen_y = ratio_y * Height;

        return new Vector2(screen_x, screen_y);
    }
    public Vector2 worldScaleToScreenScale(Vector2 scale) {
        var normalized_x = scale.X / ( view_size_x * 2.0f);
        var normalized_y = scale.Y / (-view_size_y * 2.0f);

        var screen_x = normalized_x * Width;
        var screen_y = normalized_y * Height;

        return new Vector2(screen_x, screen_y);
    }

    protected override void createView() {
        View = Matrix4.LookAt(new Vector3(position.X, position.Y, 10.0f), new Vector3(position.X, position.Y, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
    }

    protected override void createProjection() {
        var aspect_ratio = (float) Height / (float) Width;
        var width  = view_size_x * 2.0f;
        var height = width * aspect_ratio;

        view_size_y = height / 2.0f;

        Projection = Matrix4.CreateOrthographic(width, height, 0.1f, 100.0f);
    }
}