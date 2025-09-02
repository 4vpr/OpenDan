using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenDan.Graphics;

namespace OpenDan;

public sealed partial class Game
{
    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // Throttle rendering to target FPS if capped; otherwise render every frame
        if (_fpsCap is not null and > 0)
        {
            double now = _timer.Elapsed.TotalSeconds;
            if (now + 0.000_2 < _nextRenderTime)
            {
                return; // not time to render yet
            }
            while (_nextRenderTime <= now)
            {
                _nextRenderTime += RenderInterval;
            }
        }

        // Clear
        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Scene draw hook (no-op until scenes implement their own drawing)
        _scenes.Draw(1.0);

        // Simple demo draw
        if (_shader != null && _quad != null && _texture != null)
        {
            _shader.Use();

            // A simple scale over time to visualize something moving
            var t = (float)(_songTime % 1.0);
            var scale = 0.5f + 0.25f * MathF.Sin(t * MathF.Tau);

            var model = Matrix4.CreateScale(scale, scale, 1f);
            var proj = Matrix4.CreateOrthographicOffCenter(0, Size.X, Size.Y, 0, -1, 1);
            var view = Matrix4.Identity;
            var mvp = model * view * proj;

            _shader.SetMatrix4("uMvp", mvp);
            _shader.SetVector4("uTint", new Vector4(1f, 1f, 1f, 1f));

            _texture.Bind(TextureUnit.Texture0);
            _quad.Draw();
        }

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
    }
}
