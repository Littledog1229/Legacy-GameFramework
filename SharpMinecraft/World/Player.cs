using Engine.Application;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SharpMinecraft.Object;

namespace SharpMinecraft.World; 

public class Player {
    private float speed         = 5.0f;
    private readonly float sensitivity_x = 0.5f;
    private readonly float sensitivity_y = 0.5f;

    public PerspectiveCamera Camera { get; }
    public World             World  { get; }

    public Vector3 Position => Camera.CameraTransform.Position;
    public Vector3 LastPosition { get; private set; }

    public Player(World world) {
        World = world;
        Camera = new() {
            CameraTransform = {
                Position = new Vector3(0, 18, 0),
                Yaw      = 0,
                Pitch    = 0
            }
        };
        
        LastPosition = Camera.CameraTransform.Position;
    }
    
    public void update() {
        LastPosition = Camera.CameraTransform.Position;
        
        var input_x = (InputManager.keyDown(Keys.W) ? 1.0f : 0.0f) - (InputManager.keyDown(Keys.S) ? 1.0f : 0.0f);
        var input_y = (InputManager.keyDown(Keys.E) ? 1.0f : 0.0f) - (InputManager.keyDown(Keys.Q) ? 1.0f : 0.0f);
        var input_z = (InputManager.keyDown(Keys.D) ? 1.0f : 0.0f) - (InputManager.keyDown(Keys.A) ? 1.0f : 0.0f);
        
        var rotation_y = Camera.CameraTransform.Yaw;
        var rotation_z = Camera.CameraTransform.Pitch;

        var rot_input_y = (InputManager.MouseDelta.X)  * sensitivity_x;
        var rot_input_z = (-InputManager.MouseDelta.Y) * sensitivity_y;
        
        rotation_y += rot_input_y;
        rotation_z += rot_input_z;
        
        var forward = Camera.CameraTransform.Forward * input_x;
        var right   = Camera.CameraTransform.Right   * input_z;
        var up      = Camera.CameraTransform.Up      * input_y;
        
        speed = (InputManager.keyDown(Keys.LeftShift)) ? 10.0f : 5.0f;

        Camera.CameraTransform.Position += (forward + right + up) * speed *  Engine.Engine.DeltaTime;
        Camera.CameraTransform.Pitch     = rotation_z;
        Camera.CameraTransform.Yaw       = rotation_y;
    }
}