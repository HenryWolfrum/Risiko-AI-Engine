using System;
using System.IO;
using System.Numerics;
using Raylib_cs;
using RiskEngine.State;

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

        // 1. Section-Hintergrund zeichnen
        Raylib.DrawRectangleRec(bounds, MapBackgroundColor);

        if (!_isLoaded)
        {
            Raylib.DrawText("Map image not found", (int)bounds.X + 20, (int)bounds.Y + 20, 20, Color.DarkBlue);
            return;
        }

        // 2. Skalierung und Offsets des Kartenbildes berechnen
        float scale = Math.Min(bounds.Width / _mapTexture.Width, bounds.Height / _mapTexture.Height);

        float renderWidth = _mapTexture.Width * scale;
        float renderHeight = _mapTexture.Height * scale;

        float offsetX = bounds.X + (bounds.Width - renderWidth) / 2f;
        float offsetY = bounds.Y + (bounds.Height - renderHeight) / 2f;

        Vector2 renderOffset = new(offsetX, offsetY);

        // 3. Karten-Textur rendern
        Raylib.DrawTextureEx(_mapTexture, renderOffset, 0.0f, scale, Color.White);

        // 4. Dynamischen Radius & Schriftgröße passend zur skalierten Kartengröße bestimmen
        float radius = Math.Clamp(renderWidth * 0.0135f, 14f, 28f);
        int fontSize = (int)(radius * 1.2f);

        // 5. Territorien-Marker zeichnen
        var currentFrame = context.Player.CurrentFrame;

        for (int i = 0; i < TerritoryLayout.Entries.Length; i++)
        {
            var entry = TerritoryLayout.Entries[i];

            // Relative Faktoren auf die tatsächliche Render-Größe umrechnen
            float screenX = offsetX + (entry.ScaleX * renderWidth);
            float screenY = offsetY + (entry.ScaleY * renderHeight);
            Vector2 center = new(screenX, screenY);

            // Daten aus dem aktuellen Frame über den Index 'i' abrufen
            int troops = GameStateHelper.GetTerritoryTroops(currentFrame.State, i);
            Color playerColor = ReplayViewer.GetPlayerColor(GameStateHelper.GetTerritoryOwner(currentFrame.State, i));

            // A) Schwarze Outline für maximalen Kontrast zur Karte
            Raylib.DrawCircleV(center, radius + 2f, Color.Black);

            // B) Spielerfarbener Füllkreis
            Raylib.DrawCircleV(center, radius, playerColor);

            // C) Zentrierte Truppenzahl im Kreis
            string text = troops.ToString();
            int textWidth = Raylib.MeasureText(text, fontSize);

            float textX = screenX - (textWidth / 2f);
            float textY = screenY - (fontSize / 2f);

            Raylib.DrawText(text, (int)textX, (int)textY, fontSize, Color.White);
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