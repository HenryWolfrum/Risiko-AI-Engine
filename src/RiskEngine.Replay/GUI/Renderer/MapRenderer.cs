using System;
using System.IO;
using System.Numerics;
using Raylib_cs;

namespace RiskEngine.Replay.GUI;

public class MapRenderer : ISectionRenderer
{
    private Texture2D _mapTexture;
    private bool _isLoaded;
    private bool _loadAttempted;

    // Entsättigtes, sanftes Hellblau (Ocean/Water Blue)
    private static readonly Color MapBackgroundColor = new(150, 185, 210, 255);

    public void Render(Rectangle bounds, ReplayUIContext context)
    {
        if (!_loadAttempted)
        {
            LoadMapTexture();
        }

        // Section-Hintergrund in sanftem Hellblau
        Raylib.DrawRectangleRec(bounds, MapBackgroundColor);

        if (_isLoaded)
        {
            // Zentrierte und seitenverhältnistreue Skalierung
            float scale = Math.Min(bounds.Width / _mapTexture.Width, bounds.Height / _mapTexture.Height);

            float renderWidth = _mapTexture.Width * scale;
            float renderHeight = _mapTexture.Height * scale;

            float offsetX = bounds.X + (bounds.Width - renderWidth) / 2f;
            float offsetY = bounds.Y + (bounds.Height - renderHeight) / 2f;

            Raylib.DrawTextureEx(_mapTexture, new Vector2(offsetX, offsetY), 0.0f, scale, Color.White);
        }
        else
        {
            Raylib.DrawText("Map image not found", (int)bounds.X + 20, (int)bounds.Y + 20, 20, Color.DarkBlue);
        }
    }

    private void LoadMapTexture()
    {
        _loadAttempted = true;
        string imagePath = Path.Combine(AppContext.BaseDirectory, "Assets", "risk_map.png");

        if (File.Exists(imagePath))
        {
            _mapTexture = Raylib.LoadTexture(imagePath);
            _isLoaded = _mapTexture.Id != 0;
        }
    }
}