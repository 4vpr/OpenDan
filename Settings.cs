using System.Text.Json;
using System.Text.Json.Serialization;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenDan;

public sealed class Settings
{
    // Key layouts per lane count (e.g., 4 -> [D,F,J,K])
    public Dictionary<int, List<Keys>> KeyLayouts { get; init; } = new();

    // Frames per second caps. null means unlimited
    public double? IngameFpsCap { get; set; } = 60.0;
    public double? MenuFpsCap { get; set; } = 60.0;

    public bool Fullscreen { get; set; } = false;

    public IReadOnlyList<Keys> GetKeysForLanes(int lanes)
    {
        if (KeyLayouts.TryGetValue(lanes, out var list))
            return list;
        // Fallback: generate a simple left-to-right row if missing
        return GenerateDefaultLayout(lanes);
    }

    public void SetKeysForLanes(int lanes, IEnumerable<Keys> keys)
    {
        var arr = keys.ToArray();
        if (arr.Length != lanes)
            throw new ArgumentException($"Expected {lanes} keys for layout, got {arr.Length}.");
        KeyLayouts[lanes] = new List<Keys>(arr);
    }

    public static Settings Default()
    {
        var s = new Settings();
        s.KeyLayouts[4] = new List<Keys> { Keys.D, Keys.F, Keys.J, Keys.K };
        s.KeyLayouts[5] = new List<Keys> { Keys.D, Keys.F, Keys.Space, Keys.J, Keys.K };
        s.KeyLayouts[6] = new List<Keys> { Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L };
        s.KeyLayouts[7] = new List<Keys> { Keys.S, Keys.D, Keys.F, Keys.Space, Keys.J, Keys.K, Keys.L };
        s.KeyLayouts[8] = new List<Keys> { Keys.A, Keys.S, Keys.D, Keys.F, Keys.J, Keys.K, Keys.L, Keys.Semicolon };
        s.IngameFpsCap = null; // default to unlimited in game
        s.MenuFpsCap = 60.0;
        s.Fullscreen = false;
        return s;
    }

    private static IReadOnlyList<Keys> GenerateDefaultLayout(int lanes)
    {
        // Simple A..L row fallback
        Keys[] pool = { Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L, Keys.Semicolon };
        if (lanes <= pool.Length)
            return pool.Take(lanes).ToArray();
        return pool.Concat(Enumerable.Repeat(Keys.Space, Math.Max(0, lanes - pool.Length))).ToArray();
    }

    public static string DefaultPath => Path.Combine(AppContext.BaseDirectory, "settings.json");

    public static Settings LoadOrCreateDefault(string? path = null)
    {
        path ??= DefaultPath;
        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return Deserialize(json) ?? Default();
            }
        }
        catch { /* ignore and create default */ }

        var s = Default();
        TrySave(s, path);
        return s;
    }

    public void Save(string? path = null) => TrySave(this, path ?? DefaultPath);

    private static void TrySave(Settings s, string path)
    {
        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(path, Serialize(s));
        }
        catch { /* ignore write failures */ }
    }

    private static string Serialize(Settings s)
    {
        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true
        };
        opts.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Serialize(s, opts);
    }

    private static Settings? Deserialize(string json)
    {
        var opts = new JsonSerializerOptions
        {
            AllowTrailingCommas = true
        };
        opts.Converters.Add(new JsonStringEnumConverter());
        return JsonSerializer.Deserialize<Settings>(json, opts);
    }
}
