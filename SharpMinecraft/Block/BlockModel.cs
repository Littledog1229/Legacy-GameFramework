using OpenTK.Mathematics;

namespace SharpMinecraft.Block; 

public class BlockModel {
    public FaceInformation? FrontFace  { get; init; }
    public FaceInformation? BackFace   { get; init; }
    public FaceInformation? LeftFace   { get; init; }
    public FaceInformation? RightFace  { get; init; }
    public FaceInformation? TopFace    { get; init; }
    public FaceInformation? BottomFace { get; init; }

    public struct FaceInformation {
        public Face               Face     { get; init; } = Face.Top;
        public BlockModelVertex[] Vertices { get; init; } = null!;
        public uint[]             Indices  { get; init; } = null!;
        
        public FaceInformation() { }
    }
    
    public struct BlockModelVertex {
        public Vector3 Position  { get; init; } // Relative Position
        public Vector2 UV        { get; init; } // Relative UV's (0.0-1.0 for each face)
        public int     TextureID { get; init; } // Relative Texture ID of the vertex
    }

    public enum Face {
        Front,
        Back,
        Left,
        Right,
        Top,
        Bottom
    }

    public static readonly BlockModel TRANSPARENT = new() {
        FrontFace  = null,
        BackFace   = null,
        RightFace  = null,
        LeftFace   = null,
        TopFace    = null,
        BottomFace = null
    };

    public static readonly BlockModel DEFAULT = new() {
        FrontFace = new FaceInformation {
            Face = Face.Front,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f, 0.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f, 1.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(1.0f, 1.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(1.0f, 0.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        BackFace = new FaceInformation {
            Face = Face.Back,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(0.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        RightFace = new FaceInformation {
            Face = Face.Right,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(0.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(1.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(1.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        LeftFace = new FaceInformation {
            Face = Face.Left,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        TopFace = new FaceInformation {
            Face = Face.Top,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f, 1.0f, 0.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f, 1.0f, 1.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f, 1.0f, 1.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f, 1.0f, 0.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        BottomFace = new FaceInformation {
            Face = Face.Bottom,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        }
    };
    
    public static readonly BlockModel GRASS = new() {
        FrontFace = new FaceInformation {
            Face = Face.Front,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f, 0.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f, 1.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(1.0f, 1.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(1.0f, 0.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        BackFace = new FaceInformation {
            Face = Face.Back,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(0.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        RightFace = new FaceInformation {
            Face = Face.Right,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(0.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(1.0f,1.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(1.0f,0.0f,1.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        LeftFace = new FaceInformation {
            Face = Face.Left,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,1.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 0, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        TopFace = new FaceInformation {
            Face = Face.Top,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f, 1.0f, 0.0f), TextureID = 1, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f, 1.0f, 1.0f), TextureID = 1, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f, 1.0f, 1.0f), TextureID = 1, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f, 1.0f, 0.0f), TextureID = 1, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        },
        BottomFace = new FaceInformation {
            Face = Face.Bottom,
            Vertices = new BlockModelVertex[] {
                new() { Position = new Vector3(1.0f,0.0f,1.0f), TextureID = 2, UV = new Vector2(0.0f, 0.0f) },
                new() { Position = new Vector3(1.0f,0.0f,0.0f), TextureID = 2, UV = new Vector2(0.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,0.0f), TextureID = 2, UV = new Vector2(1.0f, 1.0f) },
                new() { Position = new Vector3(0.0f,0.0f,1.0f), TextureID = 2, UV = new Vector2(1.0f, 0.0f) }
            },
            Indices = new uint[] { 0, 1, 2, 0, 2, 3 }
        }
    };
}