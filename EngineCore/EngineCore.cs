using System.Reflection;

namespace Engine; 

// Core engine class, do not use unless you are using EngineCore raw (not something like ApplicationCore)
public static class EngineCore {
    public static Assembly EngineAssembly         => Assembly.GetAssembly(typeof(EngineCore))!;
    public static Assembly EngineResourceAssembly { get; private set; } = null!;
    public static Assembly ApplicationAssembly    { get; private set; } = null!;
    
    public static void setEngineResourceAssembly (Assembly assembly) => EngineResourceAssembly = assembly;
    public static void setApplicationAssembly    (Assembly assembly) => ApplicationAssembly    = assembly;
    
    public static void setPreciseDeltaTime(double delta_time) {
        Time.PreciseDeltaTime = delta_time;
        Time.DeltaTime        = (float) delta_time;
    }
    public static void setFixedDeltaTime(float fixed_delta_time) => Time.FixedDeltaTime = fixed_delta_time;

    public static void initialize()  => ENGINE_SYSTEMS.ForEach(system => system.initialize());
    public static void start()       => ENGINE_SYSTEMS.ForEach(system => system.start());
    public static void unload()      => ENGINE_SYSTEMS.ForEach(system => system.unload());
    public static void destroy()     => ENGINE_SYSTEMS.ForEach(system => system.destroy());
    public static void update()      => ENGINE_SYSTEMS.ForEach(system => system.update());
    public static void lateUpdate()  => ENGINE_SYSTEMS.ForEach(system => system.lateUpdate());
    public static void fixedUpdate() => ENGINE_SYSTEMS.ForEach(system => system.fixedUpdate());
    public static void preRender()   => ENGINE_SYSTEMS.ForEach(system => system.preRender());
    public static void render()      => ENGINE_SYSTEMS.ForEach(system => system.render());
    public static void postRender()  => ENGINE_SYSTEMS.ForEach(system => system.postRender());

    public static void registerEngineSystem<T>() where T : EngineSystem, new()  => ENGINE_SYSTEMS.Add(new T());
    public static void registerEngineSystem<T>(T system) where T : EngineSystem => ENGINE_SYSTEMS.Add(system);
    
    private static readonly List<EngineSystem> ENGINE_SYSTEMS = new();
}