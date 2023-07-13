using OpenTK.Mathematics;

namespace SharpMinecraft.World; 

public struct ChunkPosition {
    public Vector2i Position { get; init; }

    public ChunkPosition(int x, int z) { Position = new Vector2i(x, z); }

    public Vector3 toWorldPosition()  => new(Position.X * Chunk.CHUNK_SIZE_X, 0.0f, Position.Y * Chunk.CHUNK_SIZE_Z);
    public override string ToString() => $"[{Position.X}, {Position.Y}]";
    
    public static bool operator==(ChunkPosition a, ChunkPosition b) => a.Position == b.Position;
    public static bool operator!=(ChunkPosition a, ChunkPosition b) => a.Position != b.Position;

    public static ChunkPosition operator+(ChunkPosition a, ChunkPosition b) => new(a.Position.X + b.Position.X, a.Position.Y + b.Position.Y);
    public static ChunkPosition operator-(ChunkPosition a, ChunkPosition b) => new(a.Position.X - b.Position.X, a.Position.Y - b.Position.Y);
    
    public static ChunkPosition toChunkPosition(Vector3 position) {
        var sign_x = MathF.Sign(position.X);
        var sign_z = MathF.Sign(position.Z);
        
        var abs_x = MathF.Abs(position.X);
        var abs_z = MathF.Abs(position.Z);

        var rounded_x = MathF.Floor(abs_x / Chunk.CHUNK_SIZE_X);
        var rounded_z = MathF.Floor(abs_z / Chunk.CHUNK_SIZE_Z);

        var chunk_x = (int) (sign_x < 0 ? -(rounded_x + 1) : rounded_x);
        var chunk_z = (int) (sign_z < 0 ? -(rounded_z + 1) : rounded_z);

        return new ChunkPosition(chunk_x, chunk_z);
    }
    
    public static (ChunkPosition, Vector3i) toChunkLocalSpace(Vector3 position) {
        var sign_x = MathF.Sign(position.X);
        var sign_z = MathF.Sign(position.Z);
        
        var abs_x = MathF.Abs(position.X);
        var abs_z = MathF.Abs(position.Z);

        var rounded_x = MathF.Floor(abs_x / Chunk.CHUNK_SIZE_X);
        var rounded_z = MathF.Floor(abs_z / Chunk.CHUNK_SIZE_Z);

        var chunk_x = (int) (sign_x < 0 ? -(rounded_x + 1) : rounded_x);
        var chunk_z = (int) (sign_z < 0 ? -(rounded_z + 1) : rounded_z);

        var local_x = mod((int) MathF.Floor(position.X), (int) Chunk.CHUNK_SIZE_X);
        var local_y = mod((int) MathF.Floor(position.Y), (int) Chunk.CHUNK_SIZE_Y);
        var local_z = mod((int) MathF.Floor(position.Z), (int) Chunk.CHUNK_SIZE_Z);

        return (new ChunkPosition(chunk_x, chunk_z), new Vector3i(local_x, local_y, local_z));
    }
    
    private static int mod(int a, int b) => (int) (a - b * MathF.Floor(a / (float) b));
}