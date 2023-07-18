using tainicom.Aether.Physics2D.Dynamics;

namespace Engine.Physics; 

public sealed class PhysicsManager : EngineSystem {
    private static PhysicsManager Instance { get; set; } = null!;

    private readonly List<World> physics_worlds = new();

    public static World createWorld() {
        var world = new World();
        Instance.physics_worlds.Add(world);

        return world;
    }

    public static void destroyWorld(World world) => Instance.physics_worlds.Remove(world);

    public static void addWorld(World world) => Instance.physics_worlds.Add(world);
    
    public static PhysicsManager create() {
        if (Instance != null!)
            return Instance;

        Instance = new PhysicsManager();
        return Instance;
    }

    protected internal override void fixedUpdate() {
        foreach (var world in physics_worlds)
            world.Step(Time.FixedDeltaTime);
    }
}