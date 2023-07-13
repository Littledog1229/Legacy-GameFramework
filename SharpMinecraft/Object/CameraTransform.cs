using OpenTK.Mathematics;

namespace SharpMinecraft.Object; 

public sealed class CameraTransform {
    public static readonly Vector3 FORWARD = new(0.0f, 0.0f, 1.0f);
    public static readonly Vector3 RIGHT   = new(1.0f, 0.0f, 0.0f);
    public static readonly Vector3 UP      = new(0.0f, 1.0f, 0.0f);
    
    private Vector3 position;
    private float   pitch, yaw;

    public Matrix4 TransformMatrix { get; private set; }

    public Vector3    Position {
        get => position;
        set {
            if (position == value)
                return;

            position = value;
            updateMatrix();
        }
    }
    public float Pitch {
        get => pitch;
        set {
            value = MathHelper.Clamp(value, -89.9f, 89.9f);
            
            if (Math.Abs(pitch - value) < float.Epsilon)
                return;

            pitch = value;
            updateDirections();
            updateMatrix();
        }
    }
    public float Yaw {
        get => yaw;
        set {
            if (Math.Abs(yaw - value) < float.Epsilon)
                return;

            yaw = value;
            updateDirections();
            updateMatrix();
        }
    }

    public Vector3 Forward { get; private set; }
    public Vector3 Right   { get; private set; }
    public Vector3 Up      { get; private set; }

    public CameraTransform() {
        position = Vector3.Zero;
        pitch    = 0.0f;
        yaw      = 0.0f;

        updateDirections();
        updateMatrix();
    }

    public Action<Matrix4>? MatrixUpdated { get; set; }

    private void updateDirections() {
        var x = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
        var y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
        var z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

        Forward = new Vector3(x, y, z).Normalized();
        Right   = Vector3.Cross(Forward, UP).Normalized();
        Up      = Vector3.Cross(Right, Forward).Normalized();
    }
    private void updateMatrix() {
        TransformMatrix =
            Matrix4.CreateTranslation(position) *
            Matrix4.CreateRotationY(yaw) *
            Matrix4.CreateRotationZ(pitch);
        
        MatrixUpdated?.Invoke(TransformMatrix);
    }
}