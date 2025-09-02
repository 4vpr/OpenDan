using OpenTK.Graphics.OpenGL4;

namespace OpenDan.Graphics;

public sealed class QuadRenderer : IDisposable
{
    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;

    // Full-screen-space quad (0,0) to (width,height) in orthographic projection
    private static readonly float[] Vertices =
    {
        // xy      // uv
        0f, 0f,    0f, 0f,
        512f, 0f,  1f, 0f,
        512f, 512f,1f, 1f,
        0f, 512f,  0f, 1f
    };

    private static readonly uint[] Indices =
    {
        0, 1, 2,
        2, 3, 0
    };

    public QuadRenderer()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

        GL.BindVertexArray(0);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
    }
}

