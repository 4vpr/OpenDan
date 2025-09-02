using System.Globalization;

namespace OpenDan;

internal static class Program
{
    private static void Main(string[] args)
    {
        // Load settings (creates settings.json with defaults)
        var settings = Settings.LoadOrCreateDefault();

        // Optional CLI override: --fps <value> (<=0 means unlimited)
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--fps", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                if (double.TryParse(args[i + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                {
                    settings.IngameFpsCap = parsed <= 0 ? null : Math.Clamp(parsed, 1.0, 2000.0);
                }
            }
        }

        using var game = new Game(settings);
        game.Run();
    }
}
