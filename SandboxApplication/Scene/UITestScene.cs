using ApplicationCore.Render;
using ApplicationCore.UI;
using ApplicationCore.UI.Constraint;
using ApplicationCore.UI.Elements;
using Engine;
using Engine.Scenes;
using OpenTK.Mathematics;

namespace Sandbox; 

public sealed class UITestScene : Scene {
    private BoxElement box       = null!;
    private BoxElement container = null!;
    
    public override void start() {
        box = new BoxElement(Color4.Blue) {
            Transform = {
                Position = new Vector2(10, 10),
                Size = new Vector2(100, 100)
            },
            Focusable = true
        };
        
        box.Transform.addConstraint(new PixelConstraint(PixelConstraint.ConstraintSide.Right, 10));

        container = new BoxElement(Color4.Red) {
            Identifier = "Container",
            Transform = {
                Position = new Vector2(10, 120),
                Size = new Vector2(300, 300)
            },
            CanFocusChildren = true
        };

        var container_child = new BoxElement(Color4.White) {
            Transform = {
                Position = new Vector2(10, 10),
                Size = new Vector2(100, 100)
            },
            Focusable = true,
            Parent    = container
        };
        
        UIManager.Root.addChild(box);
        UIManager.Root.addChild(container);
        
        RenderManager.OnResize += UIManager.resize;
    }

    private float resize_speed = 100.0f;
    private bool inc = true;

    public override void update() {
        if (inc) {
            box.Transform.Position       += new Vector2(Time.DeltaTime * resize_speed, 0.0f);
            container.Transform.Position += new Vector2(Time.DeltaTime * resize_speed, 0.0f);
            
            if (box.Transform.Size.X < 100)
                inc = false;
        } else {
            box.Transform.Position       -= new Vector2(Time.DeltaTime * resize_speed, 0.0f);
            container.Transform.Position -= new Vector2(Time.DeltaTime * resize_speed, 0.0f);
            if (box.Transform.Position.X < 100.0f)
                inc = true;
        }
    }
}