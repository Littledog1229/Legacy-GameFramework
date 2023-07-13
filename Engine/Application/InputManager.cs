using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Application; 

public sealed class InputManager {
    internal static InputManager Instance { get; private set; } = null!;

    public static bool keyDown   (Keys key) =>  Instance.current_key_states[(int) key];
    public static bool keyUp     (Keys key) => !Instance.current_key_states[(int) key];
    public static bool keyPress  (Keys key) =>  keyDown(key) && !Instance.previous_key_states[(int) key];
    public static bool keyRelease(Keys key) => !keyDown(key) &&  Instance.previous_key_states[(int) key];
    
    public static bool mouseDown    (MouseButton button) =>  Instance.current_mouse_states[(int) button];
    public static bool mouseUp      (MouseButton button) => !Instance.current_mouse_states[(int) button];
    public static bool mousePress   (MouseButton button) =>  mouseDown(button) && !Instance.previous_mouse_states[(int) button];
    public static bool mouseRelease (MouseButton button) => !mouseDown(button) &&  Instance.previous_mouse_states[(int) button];

    public static Vector2 ScrollOffset  => Instance.scroll_offset;
    public static float   ScrollOffsetX => Instance.scroll_offset.X;
    public static float   ScrollOffsetY => Instance.scroll_offset.Y;

    public static Vector2 MouseDelta            => Instance.mouse_delta;
    public static Vector2 MousePosition         => Instance.current_mouse_position;
    public static Vector2 PreviousMousePosition => Instance.previous_mouse_position;
    public static bool    MouseMoved            => Instance.mouse_delta != Vector2.Zero;
    
    private readonly bool[] previous_key_states = new bool[(int) Keys.LastKey];
    private readonly bool[] current_key_states  = new bool[(int) Keys.LastKey];

    private readonly bool[] previous_mouse_states = new bool[(int) MouseButton.Last];
    private readonly bool[] current_mouse_states  = new bool[(int) MouseButton.Last];
    
    private Vector2 mouse_delta             = Vector2.Zero;
    private Vector2 previous_mouse_position = Vector2.Zero;
    private Vector2 current_mouse_position  = Vector2.Zero;
    
    private Vector2 scroll_offset = Vector2.Zero;

    internal static void create() {
        if (Instance != null!)
            return;

        Instance = new InputManager();
        Instance.internalSetup();
    }
    
    internal static void lateUpdate() => Instance.internalLateUpdate();

    private void internalSetup() {
        
    }

    private void internalLateUpdate() {
        Array.Copy(current_key_states,   previous_key_states,   (int) Keys.LastKey);
        Array.Copy(current_mouse_states, previous_mouse_states, (int) MouseButton.Last);
        
        scroll_offset           = Vector2.Zero;
        mouse_delta             = Vector2.Zero;
        previous_mouse_position = current_mouse_position;
    }

    internal void keyDown (KeyboardKeyEventArgs args) => current_key_states[(int) args.Key] = true;
    internal void keyUp   (KeyboardKeyEventArgs args) => current_key_states[(int) args.Key] = false;

    internal void mouseDown (MouseButtonEventArgs args) => current_mouse_states[(int) args.Button] = true;
    internal void mouseUp   (MouseButtonEventArgs args) => current_mouse_states[(int) args.Button] = false;

    internal void mouseWheel(MouseWheelEventArgs args) => scroll_offset = args.Offset;

    internal void mouseMove(MouseMoveEventArgs args) {
        current_mouse_position = args.Position;
        mouse_delta = args.Delta;
    }
}