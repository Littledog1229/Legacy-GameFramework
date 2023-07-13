using OpenTK.Mathematics;

namespace Prototype.World;

public readonly struct ChunkPosition {
    public Vector2i Position { get; }
    public Vector2i WorldPosition { get; }

    public ChunkPosition(Vector2i chunk_position) {
        Position      = chunk_position;
        WorldPosition = new Vector2i((int) (chunk_position.X * Chunk.CHUNK_SIZE_X), (int) (chunk_position.Y * Chunk.CHUNK_SIZE_Y));
    }
    
    public ChunkPosition(int x, int y) {
        Position      = new Vector2i(x, y);
        WorldPosition = new Vector2i((int) (x * Chunk.CHUNK_SIZE_X), (int) (y * Chunk.CHUNK_SIZE_Y));
    }

    public static bool operator==(ChunkPosition a, ChunkPosition b) => a.Position == b.Position;
    public static bool operator!=(ChunkPosition a, ChunkPosition b) => a.Position != b.Position;
    
    private static int mod(int a, int b) => (int) (a - b * MathF.Floor(a / (float) b));

    public static ChunkPosition worldToChunkPosition(Vector2 position) {
        var sign_x = MathF.Sign(position.X);
        var sign_y = MathF.Sign(position.Y);
        
        var abs_x = MathF.Abs(position.X);
        var abs_y = MathF.Abs(position.Y);

        var rounded_x = MathF.Floor(abs_x / Chunk.CHUNK_SIZE_X);
        var rounded_y = MathF.Floor(abs_y / Chunk.CHUNK_SIZE_Y);

        var chunk_x = (int) (sign_x < 0 ? -(rounded_x + 1) : rounded_x);
        var chunk_y = (int) (sign_y < 0 ? -(rounded_y + 1) : rounded_y);

        return new ChunkPosition(chunk_x, chunk_y);
    }
    
    public static (ChunkPosition, Vector2i) worldToLocalChunkPosition(Vector2 position) {
        var sign_x = MathF.Sign(position.X);
        var sign_y = MathF.Sign(position.Y);
        
        var abs_x = MathF.Abs(position.X);
        var abs_y = MathF.Abs(position.Y);

        var rounded_x = MathF.Floor(abs_x / Chunk.CHUNK_SIZE_X);
        var rounded_y = MathF.Floor(abs_y / Chunk.CHUNK_SIZE_Y);

        var chunk_x = (int) (sign_x < 0 ? -(rounded_x + 1) : rounded_x);
        var chunk_y = (int) (sign_y < 0 ? -(rounded_y + 1) : rounded_y);

        var local_x = mod((int) MathF.Floor(position.X), (int) Chunk.CHUNK_SIZE_X);
        var local_y = mod((int) MathF.Floor(position.Y), (int) Chunk.CHUNK_SIZE_Y);

        return (new ChunkPosition(chunk_x, chunk_y), new Vector2i(local_x, local_y));
    }
}

public sealed class Chunk {
    public const uint CHUNK_SIZE_X = 16;
    public const uint CHUNK_SIZE_Y = 16;
    
    public ChunkPosition Position { get; }

    private readonly TileObject[] tiles = new TileObject[CHUNK_SIZE_X * CHUNK_SIZE_Y];
    
    public Chunk(Vector2i chunk_position) {
        Position = new ChunkPosition(chunk_position);

        for (var x = 0; x < CHUNK_SIZE_X; x++) {
            for (var y = 0; y < CHUNK_SIZE_Y; y++) {
                this[x, y] = new TileObject(this, 0, new Vector2i(x, y));
            }
        }
    }

    public ref TileObject this[int x, int y] => ref tiles[y * CHUNK_SIZE_X + x];

    public void generateRandom() {
        var index = 1;
        if (Position.Position == Vector2i.Zero)
            index = 2;
        
        for (var x = 0; x < CHUNK_SIZE_X; x++) {
            for (var y = 0; y < CHUNK_SIZE_Y; y++) {
                this[x, y].TileIndex = (uint) (Random.Shared.Next(0, 2) > 0 ? index : 0);
            }
        }
    }

    public void initialize() {
        for (var x = 0; x < CHUNK_SIZE_X; x++) {
            for (var y = 0; y < CHUNK_SIZE_Y; y++) {
                this[x, y] = new TileObject(this, 0, new Vector2i(x, y));
            }    
        }
    }
}