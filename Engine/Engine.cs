using System.Reflection;
using Engine.Application;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;

namespace Engine; 

public static class Engine {
    public static Assembly EngineAssembly      => Assembly.GetAssembly(typeof(Engine))!;
    public static Assembly ApplicationAssembly { get; internal set; } = null!;

    public static float  DeltaTime        { get; internal set; } = 0.0f;
    public static double PreciseDeltaTime { get; internal set; } = 0.0f;

    public static MouseCursor Cursor      => WindowManager.MouseCursor;

    public static CursorState CursorState {
        get => WindowManager.MouseCursorState;
        set => WindowManager.MouseCursorState = value;

    }
    
    public static void create(Application.Application application) => WindowManager.create(application);
}