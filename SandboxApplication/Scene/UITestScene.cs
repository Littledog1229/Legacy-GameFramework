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

        new BoxElement(Color4.White) {
            Transform = {
                Position = new Vector2(10, 10),
                Size = new Vector2(100, 100)
            },
            Focusable = true,
            Parent    = container
        };

        var button = new ButtonElement {
            Identifier    = "Button",
            Transform     = {
                Position = new Vector2(10, 120),
                Size = new Vector2(200, 50)
            },
            NormalColor   = Color4.Orange,
            HoveredColor  = Color4.DarkGray,
            PressingColor = Color4.Gray,
            Focusable     = true,
            Parent        = container
        };
        
        button.Transform.addConstraint(new PixelConstraint(PixelConstraint.ConstraintSide.Right, 10));
        
        UIManager.Root.addChild(box);
        UIManager.Root.addChild(container);
        
        RenderManager.OnResize += UIManager.resize;
    }

    private float resize_speed = 100.0f;
    private bool  inc = true;

    public override void update() {
        /*if (inc) {
            container.Transform.Size += new Vector2(Time.DeltaTime * resize_speed, 0.0f);

            if (container.Transform.Size.X > (RenderManager.WindowWidth - 200))
                inc = false;
        } else {
            container.Transform.Size -= new Vector2(Time.DeltaTime * resize_speed, 0.0f);

            if (container.Transform.Size.X < 100)
                inc = true;
        }*/
    }
}