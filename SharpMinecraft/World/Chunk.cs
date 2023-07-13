using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace SharpMinecraft.World; 

public class Chunk {
    public const uint CHUNK_SIZE_X = 16;
    public const uint CHUNK_SIZE_Y = 64; //384; <- Vanilla 1.20.1 height (-64 to 320)
    public const uint CHUNK_SIZE_Z = 16;

    private bool            trigger_update = true;
    private readonly uint[] blocks;

    public Action<Chunk>? ChunkUpdated { get; set; }

    public World         World { get; }
    public ChunkPosition Position { get; }

    public Chunk(World world, ChunkPosition position) {
        World    = world;
        Position = position;
        blocks   = new uint[CHUNK_SIZE_X * CHUNK_SIZE_Y * CHUNK_SIZE_Z];
    }

    public void fillSolid() {
        var id = Position.Position is { X: >= -1, X: <= 1 } and { Y: >= -1, Y: <= 1 } ? 1 : 2;
        
        trigger_update = false;
        for (var z = 0u; z < CHUNK_SIZE_Z; z++) {
            for (var y = 0u; y < CHUNK_SIZE_Y; y++) {
                for (var x = 0u; x < CHUNK_SIZE_X; x++) {
                    this[x, y, z] = (uint) id;
                }
            }
        }

        ChunkUpdated?.Invoke(this);
        trigger_update = true;
    }
    public void fillGarbage() {
        trigger_update = false;
        for (var z = 0u; z < CHUNK_SIZE_Z; z++) {
            for (var y = 0u; y < CHUNK_SIZE_Y; y++) {
                for (var x = 0u; x < CHUNK_SIZE_X; x++) {
                    this[x, y, z] = (uint) Random.Shared.Next(0, 6);
                }
            }
        }

        ChunkUpdated?.Invoke(this);
        trigger_update = true;
    }

    private float scale     = 3f;
    private int   base_line = 30;
    private int   amplitude = 10;
    
    public void generateTerrain() {
        var heightmap = new int[CHUNK_SIZE_X * CHUNK_SIZE_Z];
        var world_pos = Position.toWorldPosition();

        var noise = World.Noise;
        
        for (var z = 0; z < CHUNK_SIZE_Z; z++) {
            for (var x = 0; x < CHUNK_SIZE_X; x++) {
                var noise_pos_x = (world_pos.X + x) * scale;
                var noise_pos_z = (world_pos.Z + z) * scale;

                heightmap[z * CHUNK_SIZE_X + x] = base_line + (int) MathF.Floor(noise.GetNoise(noise_pos_x, noise_pos_z) * amplitude);
            }
        }
        
        trigger_update = false;
        
        for (var z = 0; z < CHUNK_SIZE_Z; z++) {
            for (var x = 0; x < CHUNK_SIZE_X; x++) {
                for (var y = 0; y < heightmap[z * CHUNK_SIZE_X + x]; y++) {
                    this[x, y, z] = (y >= heightmap[z * CHUNK_SIZE_X + x] - 3) ? 3u : 2u;
                }
            }
        }
        
        for (var z = 0; z < CHUNK_SIZE_Z; z++) {
            for (var x = 0; x < CHUNK_SIZE_X; x++) {
                this[x, heightmap[z * CHUNK_SIZE_X + x], z] = 4;
            }
        }

        for (var z = 0; z < CHUNK_SIZE_Z; z++) {
            for (var x = 0; x < CHUNK_SIZE_X; x++) {
                this[x, 0, z] = 1;
            }
        }
        
        trigger_update = true;

    }
    
    public uint this[uint x, uint y, uint z] {
        get => blocks[(z * CHUNK_SIZE_Y * CHUNK_SIZE_X) + (y * CHUNK_SIZE_X) + x];
        set {
            blocks[(z * CHUNK_SIZE_Y * CHUNK_SIZE_X) + (y * CHUNK_SIZE_X) + x] = value;
            if (trigger_update)
                ChunkUpdated?.Invoke(this);
        }
    }
    public uint this[int x, int y, int z] {
        get => this[(uint) x, (uint) y, (uint) z];
        set => this[(uint) x, (uint) y, (uint) z] = value;
    }

    public static bool isWithinChunkBounds(int x, int y, int z) {
        return 
            x >= 0 && x < CHUNK_SIZE_X && 
            y >= 0 && y < CHUNK_SIZE_Y && 
            z >= 0 && z < CHUNK_SIZE_Z;
    }
    public static bool isWithinChunkBounds(Vector3i value) => isWithinChunkBounds(value.X, value.Y, value.Z);
}