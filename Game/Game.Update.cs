using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenDan;

public sealed partial class Game
{
    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        // Basic input shortcuts
        if (KeyboardState.IsKeyPressed(Keys.Escape))
        {
            Close();
            return;
        }

        // Toggle fullscreen with Alt+Enter
        if (KeyboardState.IsKeyPressed(Keys.Enter) && (KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt)))
        {
            WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
        }

        // Fixed-timestep update loop at 1000 Hz
        _accumulator += args.Time;
        int safety = 0;
        while (_accumulator >= FixedDt && safety < 10)
        {
            FixedUpdate(FixedDt);
            _accumulator -= FixedDt;
            safety++;
        }
    }

    private void FixedUpdate(double dt)
    {
        // Advance gameplay clock; hook your simulation here
        _songTime += dt;
        _scenes.Update(dt);
    }
}
