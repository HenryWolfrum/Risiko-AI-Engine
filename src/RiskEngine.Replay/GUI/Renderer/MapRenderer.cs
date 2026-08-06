using System.Numerics;
using Raylib_cs;
using RiskEngine.Replay;

public sealed class MapRenderer
{
    private const float MapScale = 0.28f;

    private const int BackgroundPadding = 20;
    private const int BorderThickness = 2;

    private Texture2D _mapTexture;

    public void Initialize()
    {
        _mapTexture = Raylib.LoadTexture("Assets/risk_map_image.png");
    }

    public void Draw(ReplayFrame frame, Rectangle area)
    {
        float mapWidth = _mapTexture.Width * MapScale;
        float mapHeight = _mapTexture.Height * MapScale;

        // Karte innerhalb des zugewiesenen Bereichs zentrieren
        float x = area.X + (area.Width - mapWidth) / 2f;
        float y = area.Y + (area.Height - mapHeight) / 2f;

        Rectangle background = new Rectangle(
            x - BackgroundPadding,
            y - BackgroundPadding,
            mapWidth + BackgroundPadding * 2,
            mapHeight + BackgroundPadding * 2);

        Raylib.DrawRectangleRec(background, Color.White);
        Raylib.DrawRectangleLinesEx(background, BorderThickness, Color.Black);

        Raylib.DrawTextureEx(_mapTexture, new Vector2(x, y), 0f, MapScale, Color.White);
    }

    public void Shutdown()
    {
        Raylib.UnloadTexture(_mapTexture);
    }
}