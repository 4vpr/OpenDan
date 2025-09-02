// 남들보기에 부끄럽지 않은 코드를 쓰자

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenDan.Graphics;
using OpenDan.Scene;

namespace OpenDan;

public sealed partial class Game : GameWindow
{
    private readonly Settings _settings;
    private readonly double? _fpsCap;

    // Fixed update rate for rhythm logic (1 kHz)
    private const double FixedHz = 1000.0;
    private const double FixedDt = 1.0 / FixedHz;
    private double _accumulator;
    private double _songTime;

    // Minimal render resources
    private Shader? _shader;
    private Texture2D? _texture;
    private QuadRenderer? _quad;

    private readonly System.Diagnostics.Stopwatch _timer = System.Diagnostics.Stopwatch.StartNew();
    private double _nextRenderTime;
    private double RenderInterval => _fpsCap is null or <= 0 ? 0 : 1.0 / Math.Max(1.0, _fpsCap.Value);

    // Simple scene manager
    private readonly SceneManager _scenes = new();

    public Game(Settings settings) : base(
        new GameWindowSettings
        {
            // Let UpdateFrame run freely; we control fixed-step with accumulator
            UpdateFrequency = 0.0
        },
        new NativeWindowSettings
        {
            ClientSize = new Vector2i(1280, 720),
            Title = "OpenDan",
            Flags = ContextFlags.ForwardCompatible,
            Profile = ContextProfile.Core,
            APIVersion = new Version(3, 3)
        })
    {
        _settings = settings;
        _fpsCap = settings.IngameFpsCap;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        VSync = VSyncMode.Off; // honor custom FPS

        GL.ClearColor(0.06f, 0.07f, 0.10f, 1f);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _shader = new Shader(VertexSource, FragmentSource);
        _quad = new QuadRenderer();

        // Try to load an image from Assets/sample.png (optional)
        var assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "sample.png");
        if (File.Exists(assetPath))
        {
            _texture = Texture2D.LoadFromFile(assetPath, true);
        }
        else
        {
            // Fallback: 2x2 checkerboard
            _texture = Texture2D.FromCheckerboard(2, 2);
        }
        _timer.Restart();
        _nextRenderTime = 0.0;

        // Apply fullscreen preference
        WindowState = _settings.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;

        // Boot to main menu (placeholder)
        _scenes.Set(new MainMenu());
    }

    // Update and Render loops are split into partial files:
    // - Game.Update.cs
    // - Game.Render.cs

    protected override void OnUnload()
    {
        base.OnUnload();
        _texture?.Dispose();
        _quad?.Dispose();
        _shader?.Dispose();
    }

    private const string VertexSource = @"#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aUV;

uniform mat4 uMvp;
out vec2 vUV;

void main()
{
    vUV = aUV;
    gl_Position = uMvp * vec4(aPos, 0.0, 1.0);
}";

    private const string FragmentSource = @"#version 330 core
in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uTex;
uniform vec4 uTint;

void main()
{
    vec4 tex = texture(uTex, vUV);
    FragColor = tex * uTint;
}";
}
