using System.Diagnostics;
using System.Reflection;
using Engine.Utility;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ApplicationCore.Render; 

public sealed class Shader : GLObject {
    public override ObjectLabelIdentifier LabelIdentifier => ObjectLabelIdentifier.Program;
    
    public string Identifier { get; }
    
    public void bind() => GL.UseProgram(ObjectHandle);
    public override void destroy() {
        GL.DeleteProgram(ObjectHandle);
        RenderManager.deregisterShader(Identifier);
    }

    public Shader(string identifier, string filepath, Assembly? assembly = null) {
        createShader(FileUtility.readEmbeddedRaw(filepath, assembly));
        Identifier = identifier;
        RenderManager.registerShader(identifier, this);        
    }

    public void setUniform(string uniform, float   value)                        => GL.Uniform1(uniform_locations[uniform], value);
    public void setUniform(string uniform, int     value)                        => GL.Uniform1(uniform_locations[uniform], value);
    public void setUniform(string uniform, Vector2 value)                        => GL.Uniform2(uniform_locations[uniform], value);
    public void setUniform(string uniform, Vector3 value)                        => GL.Uniform3(uniform_locations[uniform], value);
    public void setUniform(string uniform, Vector4 value)                        => GL.Uniform4(uniform_locations[uniform], value);
    public void setUniform(string uniform, Color4  value)                        => GL.Uniform4(uniform_locations[uniform], value);
    public void setUniform(string uniform, Matrix4 value, bool transpose = false) => GL.UniformMatrix4(uniform_locations[uniform], transpose, ref value);
    
    public void setUniform(string uniform, ref float   value)                        => GL.Uniform1(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref int     value)                        => GL.Uniform1(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref Vector2 value)                        => GL.Uniform2(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref Vector3 value)                        => GL.Uniform3(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref Vector4 value)                        => GL.Uniform4(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref Color4  value)                        => GL.Uniform4(uniform_locations[uniform], value);
    public void setUniform(string uniform, ref Matrix4 value, bool transpose = true) => GL.UniformMatrix4(uniform_locations[uniform], transpose, ref value);
    
    public void setUniformArray(string uniform, float[] value) => GL.Uniform1(uniform_locations[$"{uniform}[0]"], value.Length, value);
    public void setUniformArray(string uniform, int[]   value) => GL.Uniform1(uniform_locations[$"{uniform}[0]"], value.Length, value);
    
    private readonly Dictionary<string, int> uniform_locations = new();

    private void getUniforms() {
        GL.GetProgram(ObjectHandle, GetProgramParameterName.ActiveUniforms, out var uniform_count);
#if DEBUG
        Console.WriteLine($"Program Uniform Count: {uniform_count}");
#endif
        
        for (var i = 0; i < uniform_count; i++) {
#if DEBUG
            var uniform_name   = GL.GetActiveUniform(ObjectHandle, i, out var size, out var type)!;
#else
            var uniform_name   = GL.GetActiveUniform(ObjectHandle, i, out _, out _)!;
#endif
            var uniform_location = GL.GetUniformLocation(ObjectHandle, uniform_name);
            uniform_locations.Add(uniform_name, GL.GetUniformLocation(ObjectHandle, uniform_name));

#if DEBUG
            Console.WriteLine($" . Uniform: [{i} = {uniform_name} => {uniform_location}] with properties: [{type} of size {size}]");
#endif
        }
    }
    private void createShader(string raw_text) {
        var (vertex_source, fragment_source) = getShaderSource(raw_text);
        
        var vertex_shader   = GL.CreateShader(ShaderType.VertexShader);
        var fragment_shader = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vertex_shader,   vertex_source);
        GL.ShaderSource(fragment_shader, fragment_source);
        
        GL.CompileShader(vertex_shader);
        GL.GetShader(vertex_shader, ShaderParameter.CompileStatus, out var success);
        if (success == 0) {
            var info_log = GL.GetShaderInfoLog(vertex_shader);
            Console.WriteLine($"Vertex Shader Compilation: {info_log}");
        }

        GL.CompileShader(fragment_shader);
        GL.GetShader(fragment_shader, ShaderParameter.CompileStatus, out success);
        if (success == 0) {
            var info_log = GL.GetShaderInfoLog(fragment_shader);
            Console.WriteLine($"Fragment Shader Compilation: {info_log}");
        }

        ObjectHandle = GL.CreateProgram();
        
        GL.AttachShader(ObjectHandle, vertex_shader);
        GL.AttachShader(ObjectHandle, fragment_shader);
        
        GL.LinkProgram(ObjectHandle);
        
        GL.GetProgram(ObjectHandle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0) {
            var info_log = GL.GetProgramInfoLog(ObjectHandle);
            Console.WriteLine($"Shader Compilation Failure: {info_log}");
        }
        
        getUniforms();
        
        GL.DetachShader(ObjectHandle, vertex_shader);
        GL.DetachShader(ObjectHandle, fragment_shader);
        GL.DeleteShader(vertex_shader);
        GL.DeleteShader(fragment_shader);
    }
    
    private static (string vertex_source, string fragment_source) getShaderSource(string raw_text) {
        var shaders = raw_text.Split("#type ");

        var vertex_source   = string.Empty;
        var fragment_source = string.Empty;
        
        for (var i = 1; i < shaders.Length; i++) {
            var index = shaders[i].IndexOf("\r\n", StringComparison.Ordinal);
            var shader_type = shaders[i][..index];

            switch (shader_type) {
                case "vertex":
                    vertex_source = shaders[i].Remove(0, index + 2);
                    break;
                case "fragment":
                    fragment_source = shaders[i].Remove(0, index + 2);
                    break;
                
            }
        }

        return (vertex_source, fragment_source);
    }
    
    internal void internalDestroy() => GL.DeleteProgram(ObjectHandle);
}