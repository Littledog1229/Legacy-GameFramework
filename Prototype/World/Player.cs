using Engine.Application;
using Engine.Render.Batch;
using Engine.Render.Camera;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Prototype.World; 

public sealed class Player {
    public OrthoCamera Camera           { get; private set; }
    public Vector2     PreviousPosition { get; private set; } = Vector2.Zero;
    public Vector2     Position         { get; set; }
    public int         RenderDistance   { get; private set; } = 2;

    private readonly float speed           = 5.0f;

    public Player(Vector2 position) {
        Position = position;
        Camera = new OrthoCamera();
        Camera.initialize();
    }

    public void update() {
        PreviousPosition = Position;

        var left  = InputManager.keyDown(Keys.A) ? 1 : 0;
        var right = InputManager.keyDown(Keys.D) ? 1 : 0;
        var up    = InputManager.keyDown(Keys.W) ? 1 : 0;
        var down  = InputManager.keyDown(Keys.S) ? 1 : 0;
        
        var movement = new Vector2(right - left, up - down);
        if (movement != Vector2.Zero)
            movement.Normalized();

        var scroll = InputManager.ScrollOffsetY;
        if (scroll != 0)
            Camera.ViewSizeX -= scroll * 0.5f;

        Position        += movement * speed * Engine.Engine.DeltaTime;
        Camera.Position  = Position;
    }

    public void render(ShapeBatch batch) {
        batch.drawBox(Position, Color4.Orange);
    }
}