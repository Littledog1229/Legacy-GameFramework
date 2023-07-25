using System.Reflection;
using System.Runtime.InteropServices;
using Engine.Scenes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SharpFont;

namespace Sandbox;


// Texture packing based on https://blackpawn.com/texts/lightmaps/default.html?

public sealed class FontTestingScene : Scene {
    private Dictionary<char, Character> characters = new();

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool SetDllDirectory(string path);
    
    public override void start() {
        // This hacky workaround makes me sad, but this is really the only FreeType port that I know of (Stolen directly from the SharpFont example)
        // I may want to follow in its footsteps and start managing my DLL's rather than having it automatically done by Nuget
        //  . With that, I may also want to go ahead and include CPU platforms so that I can verify everything works cross platform
        /*int p = (int)Environment.OSVersion.Platform;
        if (p != 4 && p != 6 && p != 128)
        {
            //Thanks StackOverflow! http://stackoverflow.com/a/2594135/1122135
            string path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            path = Path.Combine(path, IntPtr.Size == 8 ? "x64" : "x64");
            if (!SetDllDirectory(path))
                throw new System.ComponentModel.Win32Exception();
        }*/

        
        var library = new Library();
        var face    = new Face(library, "Resource/Font/arial.ttf", 0);
        
        face.SetPixelSizes(0, 28);
        face.LoadChar('X', LoadFlags.Render, LoadTarget.Normal);

        var width  = face.Glyph.Bitmap.Width;
        var height = face.Glyph.Bitmap.Rows;

        var bitmap_data = new byte[width * height];
        Marshal.Copy(face.Glyph.Bitmap.Buffer, bitmap_data, 0, width * height);

        // Textures are expected to be stored in increments of 4 bytes, which we dont do for font bitmaps
        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
        
        var texture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, texture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, width, height, 0, PixelFormat.Red, PixelType.UnsignedByte, bitmap_data);
        var edge_clamp = TextureWrapMode.ClampToEdge;
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) edge_clamp);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) edge_clamp);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        
        face.Dispose();
        library.Dispose();
    }

    struct Character {
        public uint     TextureID { get; init; }
        public Vector2i Size      { get; init; }
        public Vector2i Bearing   { get; init; }
        public uint     Advance   { get; init; }
    }

    class TextureNode {
        public TextureNode[] Children    { get;      } = new TextureNode[2];
        public Vector2i      BoundingBox { get; set; }

        //public TextureNode insert() { }
        
        private struct Bounds {
            public Vector2i Position;
            public Vector2i Scale;
        }
    }
}