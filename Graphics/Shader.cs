using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenDan.Graphics;

public sealed class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexSource, string fragmentSource)
    {
        int v = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(v, vertexSource);
        GL.CompileShader(v);
        GL.GetShader(v, ShaderParameter.CompileStatus, out int vStatus);
        if (vStatus == 0)
        {
            var log = GL.GetShaderInfoLog(v);
            throw new InvalidOperationException($"Vertex shader compile error:\n{log}");
        }

        int f = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(f, fragmentSource);
        GL.CompileShader(f);
        GL.GetShader(f, ShaderParameter.CompileStatus, out int fStatus);
        if (fStatus == 0)
        {
            var log = GL.GetShaderInfoLog(f);
            throw new InvalidOperationException($"Fragment shader compile error:\n{log}");
        }

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, v);
        GL.AttachShader(Handle, f);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int pStatus);
        if (pStatus == 0)
        {
            var log = GL.GetProgramInfoLog(Handle);
            throw new InvalidOperationException($"Program link error:\n{log}");
        }

        GL.DetachShader(Handle, v);
        GL.DetachShader(Handle, f);
        GL.DeleteShader(v);
        GL.DeleteShader(f);
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        if (loc != -1)
        {
            GL.UniformMatrix4(loc, false, ref value);
        }
    }

    public void SetVector4(string name, Vector4 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        if (loc != -1)
        {
            GL.Uniform4(loc, value);
        }
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }
}

